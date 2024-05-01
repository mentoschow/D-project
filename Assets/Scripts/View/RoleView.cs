using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleView : MonoBehaviour
{
    public RoleType characterType;
    [SerializeField]
    private SpriteRenderer roleSp;
    [SerializeField]
    private float moveSpeed = 0.01f;
    [SerializeField]
    private float slopeCheckDis;
    [SerializeField]
    private LayerMask ground;
    [SerializeField]
    private LayerMask wall;
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D trigger;
    public MoveVector moveVec = MoveVector.None;
    private Vector2 colliderSize;
    private Vector2 slopeNormalPerp;
    private bool isOnSlope;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private GameObject triggerObj;
    private GameObject tips;
    private float originScaleX;  // œÚ”“
    private string colliderEquipmentID = null;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        trigger = GetComponent<BoxCollider2D>();
        tips = transform.Find("tips")?.gameObject;
        tips.SetActive(false);
        originScaleX = roleSp.transform.localScale.x;
        colliderSize = capsuleCollider.size;
    }

    private void FixedUpdate()
    {
        CheckSlope();
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        float realMoveSpeed = moveSpeed;
        switch (moveVec)
        {
            case MoveVector.None:
                realMoveSpeed *= 0;
                rigidBody.sharedMaterial = fullFriction;
                break;
            case MoveVector.Left:
                realMoveSpeed *= -1;
                rigidBody.sharedMaterial = noFriction;
                roleSp.transform.localScale = new Vector3(-originScaleX, roleSp.transform.localScale.y, 1);
                break;
            case MoveVector.Right:
                realMoveSpeed *= 1;
                rigidBody.sharedMaterial = noFriction;
                roleSp.transform.localScale = new Vector3(originScaleX, roleSp.transform.localScale.y, 1);
                break;
        }
        if (isOnSlope)
        {
            rigidBody.velocity = new Vector2(-slopeNormalPerp.x * realMoveSpeed, -slopeNormalPerp.y * realMoveSpeed);
        }
        else
        {
            rigidBody.velocity = new Vector2(realMoveSpeed, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Equipment" && GameDataProxy.Instance.canOperate)
        {
            tips.SetActive(true);
            triggerObj = collision.gameObject;
            colliderEquipmentID = triggerObj.GetComponent<EquipmentView>().equipmentID;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        tips.SetActive(false);
        colliderEquipmentID = null;
    }

    private void CheckSlope()
    {
        Vector2 checkPos = transform.position - new Vector3(0f, colliderSize.y / 2);
        SlopeCheckHorizontal(checkPos);
        CheckSlopeVertical(checkPos);
    }

    private void CheckSlopeVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDis, ground);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;
        }
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        
    }

    public void InteractWithEquipment()
    {
        if (colliderEquipmentID != null)
        {
            MessageManager.Instance.Send(MessageDefine.InteractWithEquipment, new MessageData(colliderEquipmentID));
        }
    }
}

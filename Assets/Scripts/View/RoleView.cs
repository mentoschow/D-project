using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleView : MonoBehaviour
{
    public RoleType roleType;
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
    [SerializeField]
    private TextMesh tips;

    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    public MoveVector moveVec = MoveVector.None;
    private Vector2 colliderSize;
    private Vector2 slopeNormalPerp;
    private bool isOnSlope;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private float originScaleX;  // ����
    private EquipmentView colliderEquipment = null;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        tips?.gameObject.SetActive(false);
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
                roleSp.transform.localScale = new Vector3(originScaleX, roleSp.transform.localScale.y, 1);
                break;
            case MoveVector.Right:
                realMoveSpeed *= 1;
                rigidBody.sharedMaterial = noFriction;
                roleSp.transform.localScale = new Vector3(-originScaleX, roleSp.transform.localScale.y, 1);
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
        UpdateCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        UpdateCollision(collision);
    }

    private void UpdateCollision(Collider2D collision)
    {
        if (collision.tag == "Equipment" && GameDataProxy.Instance.canOperate)
        {
            var temp = collision.gameObject.GetComponent<EquipmentView>();
            if (temp != null)
            {
                colliderEquipment = temp;
            }
            if (colliderEquipment != null)
            {
                bool interactive = colliderEquipment.interactive;
                var config = ConfigController.Instance.GetEquipmentConfig(colliderEquipment.equipmentID);
                if (config != null)
                {
                    if (config.mustDoneEpisodeID?.Count > 0)
                    {
                        foreach (var id in config.mustDoneEpisodeID)
                        {
                            if (!GameDataProxy.Instance.finishedEpisode.Contains(id))
                            {
                                interactive = false;
                                break;
                            }
                        }
                    }
                    tips.text = config.name + "\n��[E]����";
                }
                tips?.gameObject.SetActive(interactive);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        tips?.gameObject.SetActive(false);
        colliderEquipment = null;
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
        if (colliderEquipment != null)
        {
            if (!colliderEquipment.interactive)
            {
                return;
            }
            var doorType = colliderEquipment.doorType;
            if (doorType != DoorType.None)
            {
                switch (doorType)
                {
                    case DoorType.LibraryOut:
                        SceneController.Instance.ChangeScene(StageType.LibraryIn, StageType.LibraryOut);
                        break;
                    case DoorType.LibraryInLeft:
                        SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.LibraryIn);
                        break;
                    case DoorType.LibraryInRight:
                        SceneController.Instance.ChangeScene(StageType.Passage, StageType.LibraryIn);
                        break;
                    case DoorType.PassageInLeft:
                        SceneController.Instance.ChangeScene(StageType.LibraryIn, StageType.Passage);
                        break;
                    case DoorType.PassageInRight:
                        SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.Passage);
                        break;
                    case DoorType.BoxRoomInLeft:
                        SceneController.Instance.ChangeScene(StageType.Passage, StageType.BoxRoom);
                        break;
                    case DoorType.BoxRoomInRight:
                        SceneController.Instance.ChangeScene(StageType.SecretRoom_Now, StageType.BoxRoom);
                        break;
                    case DoorType.SecretRoomNowInRight:
                        SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Now);
                        break;
                }
            }
            else
            {
                MessageManager.Instance.Send(MessageDefine.InteractWithEquipment, new MessageData(colliderEquipment.equipmentID));
            }
        }
    }
}

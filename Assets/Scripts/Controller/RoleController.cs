using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleController : MonoBehaviour
{
    public SpriteRenderer roleSp;
    private CharacterController controller;
    public float moveSpeed = 0.01f;

    private MoveVector moveVec = MoveVector.None;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (GameDataProxy.Instance.canMainRoleMove)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // 向左移动
                moveVec = MoveVector.Left;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                // 向右移动
                moveVec = MoveVector.Right;
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                moveVec = MoveVector.None;
            }
            switch (moveVec)
            {
                case MoveVector.None:
                    controller.SimpleMove(new Vector3(0, 0, 0));
                    break;
                case MoveVector.Left:
                    controller.SimpleMove(new Vector3(-moveSpeed, 0, 0));
                    break;
                case MoveVector.Right:
                    controller.SimpleMove(new Vector3(moveSpeed, 0, 0));
                    break;
            }
        }
    }
}

public enum MoveVector
{
    None,  // 原地不动
    Left,
    Right,
}

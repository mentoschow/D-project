using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleController : MonoSingleton<RoleController>
{
    public RoleType curRoleType = RoleType.MainRoleGirl;
    public RoleView curRoleView = null;
    private List<RoleView> roleViewList = new List<RoleView>();

    private void Start()
    {
        foreach(Transform child in transform)
        {
            var view = child.GetComponent<RoleView>();
            if (view != null)
            {
                roleViewList.Add(view);
            }
        }
        UpdateRoleEnable();
    }

    public void ChangeRole(RoleType type)
    {
        curRoleType = type;
        UpdateRoleEnable();
    }

    private void UpdateRoleEnable()
    {
        foreach (RoleView view in roleViewList)
        {
            if (view.characterType == curRoleType)
            {
                view.gameObject.SetActive(true);
                curRoleView = view;
            }
            else
            {
                view.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (curRoleView == null)
        {
            Debug.LogError("roleview is null");
            return;
        }
        if (GameDataProxy.Instance.canMainRoleMove)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // �����ƶ�
                curRoleView.moveVec = MoveVector.Left;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                // �����ƶ�
                curRoleView.moveVec = MoveVector.Right;
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                curRoleView.moveVec = MoveVector.None;
            }
        }
    }

}
public enum MoveVector
{
    None,  // ԭ�ز���
    Left,
    Right,
}

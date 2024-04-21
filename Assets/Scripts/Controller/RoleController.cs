using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleController : MonoSingleton<RoleController>
{
    public GameObject girlRoleView;
    public GameObject boyRoleView;
    private Dictionary<RoleType, RoleView> roleViewDic = new Dictionary<RoleType, RoleView>();
    private RoleType curRoleType = RoleType.MainRoleGirl;
    public RoleView curRoleView;

    private void Start()
    {
        ChangeRole(RoleType.MainRoleGirl);
    }

    public void ChangeRole(RoleType type)
    {
        curRoleType = type;
        if (!roleViewDic.ContainsKey(type))
        {
            CreateRole(type);
        }
        curRoleView = roleViewDic[type];
        foreach (var (key, view) in roleViewDic)
        {
            if (key == type)
            {
                view.gameObject.SetActive(true);
            }
            else
            {
                view.gameObject.SetActive(false);
            }
        }
    }

    private void CreateRole(RoleType type)
    {
        GameObject obj = null;
        if (type == RoleType.MainRoleGirl)
        {
            obj = girlRoleView;
        }
        else if (type == RoleType.MainRoleBoy)
        {
            obj = boyRoleView;
        }
        var view = Instantiate(obj).GetComponent<RoleView>();
        view.transform.SetParent(transform);
        roleViewDic[type] = view;
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
        if (GameDataProxy.Instance.canOperate)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // 向左移动
                curRoleView.moveVec = MoveVector.Left;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                // 向右移动
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
    None,  // 原地不动
    Left,
    Right,
}

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
}

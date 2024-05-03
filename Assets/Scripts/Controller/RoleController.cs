using DG.Tweening;
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
        curRoleView.transform.position = new Vector2(-13f, -0.216f);
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                // ����
                curRoleView.InteractWithEquipment();
            }
        }
        else
        {
            curRoleView.moveVec = MoveVector.None;
        }
    }

    public void SetRolePos(float posX)
    {
        curRoleView.transform.position = new Vector2(posX, 0);
    }

    public void PlayAutoMove(string ID)
    {
        var config = ConfigController.Instance.GetCharacterAutoMoveConfig(ID);
        if (config != null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(curRoleView.transform.DOMoveX(config.posX, config.duration)).AppendCallback(() =>
            {
                GameLineNode node = new GameLineNode();
                node.type = GameNodeType.CharacterMove;
                node.ID = ID;
                MessageManager.Instance.Send(MessageDefine.StageStart, new MessageData(node));
            });
        }
    }
}
public enum MoveVector
{
    None,  // ԭ�ز���
    Left,
    Right,
}

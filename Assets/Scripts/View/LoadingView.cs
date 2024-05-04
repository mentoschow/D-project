using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour
{
    [SerializeField]
    private Text content;
    [SerializeField]
    private float time = 1.5f;

    private TransitionType curType;

    void Start()
    {
        
    }

    public void PlayTransition(string str)
    {
        content.text = str;
        Invoke("PlayTransitionOver", time);
    }

    public void PlayTransition(TransitionType type)
    {
        curType = type;
        switch (type)
        {
            case TransitionType.Blackout:
                content.text = "";
                break;
            case TransitionType.None:
                content.text = "";
                break;
            case TransitionType.ChangeToBoy:
            case TransitionType.ChangeToBoy2:
                content.text = "���ң���ȥ��";
                RoleController.Instance.ChangeRole(RoleType.MainRoleBoy);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Pass, StageType.BoxRoom, false);
                break;
            case TransitionType.ChangeToGirl:
                content.text = "�ؼ䣨���ڣ�";
                GameDataProxy.Instance.canPlayJiguangui = true;
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToGirl2:
                content.text = "�ؼ䣨���ڣ�";
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                //SceneController.Instance.UpdateDoor(DoorType.BoxRoomInRight, true);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToBoy3:
                content.text = "���ң���ȥ��";
                RoleController.Instance.ChangeRole(RoleType.MainRoleBoy);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Pass, StageType.SecretRoom_Now, false);
                break;
            case TransitionType.ChangeToGirl3:
                content.text = "���ң����ڣ�";
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToLibraryOut:
                content.text = "ͼ�����";
                SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.SecretRoom_Now, false);
                break;
            case TransitionType.ChangeToSecretRoom_Now:
                content.text = "���ң����ڣ�";
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Now, StageType.LibraryOut, false);
                break;
        }
        Invoke("TriggerNextNode", time);
    }

    private void PlayTransitionOver()
    {
        gameObject.SetActive(false);
    }

    private void TriggerNextNode()
    {
        gameObject.SetActive(false);
        GameLineNode node = new GameLineNode();
        node.type = GameNodeType.Transition;
        node.ID = curType.ToString();
        MessageManager.Instance.Send(MessageDefine.GameLineNodeDone, new MessageData(node));
    }
}

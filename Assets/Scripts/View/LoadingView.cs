using DG.Tweening;
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
    [SerializeField]
    private Image bg;

    private TransitionType curType;

    void Awake()
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
                AudioController.Instance.PlayAudioEffect(AudioType.BlackOut);
                AudioController.Instance.PlayBgm(AudioType.NowBgm);
                content.text = "";
                break;
            case TransitionType.None:
                content.text = "";
                break;
            case TransitionType.ChangeToBoy:
            case TransitionType.ChangeToBoy2:
                AudioController.Instance.PlayAudioEffect(AudioType.Telegraph);
                content.text = "√‹ “";
                AudioController.Instance.PlayBgm(AudioType.PassBgm);
                RoleController.Instance.ChangeRole(RoleType.MainRoleBoy);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Pass, StageType.BoxRoom, false);
                break;
            case TransitionType.ChangeToGirl:
                AudioController.Instance.PlayAudioEffect(AudioType.Telegraph);
                content.text = "≤ÿº‰";
                AudioController.Instance.PlayBgm(AudioType.NowBgm);
                GameDataProxy.Instance.canPlayJiguangui = true;
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToGirl2:
                AudioController.Instance.PlayAudioEffect(AudioType.Telegraph);
                content.text = "≤ÿº‰";
                AudioController.Instance.PlayBgm(AudioType.NowBgm);
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                //SceneController.Instance.UpdateDoor(DoorType.BoxRoomInRight, true);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToBoy3:
                AudioController.Instance.PlayAudioEffect(AudioType.Telegraph);
                content.text = "√‹ “";
                AudioController.Instance.PlayBgm(AudioType.PassBgm);
                RoleController.Instance.ChangeRole(RoleType.MainRoleBoy);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Pass, StageType.SecretRoom_Now, false);
                break;
            case TransitionType.ChangeToGirl3:
                AudioController.Instance.PlayAudioEffect(AudioType.Telegraph);
                content.text = "√‹ “";
                AudioController.Instance.PlayBgm(AudioType.NowBgm);
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToLibraryOut:
                content.text = "Õº Èπ›Õ‚";
                SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.SecretRoom_Now, false);
                break;
            case TransitionType.ChangeToSecretRoom_Now:
                content.text = "√‹ “";
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Now, StageType.LibraryOut, false);
                break;
            case TransitionType.ChangeToSecretRoom_Now1:
                content.text = "√‹ “";
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Now, StageType.BoxRoom, false);
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

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

    void Update()
    {
        
    }

    public void PlayTransition(string str)
    {
        content.text = str;
        Invoke("PlayTransitionOver", time);
    }

    public void PlayTransition(TransitionType type)
    {
        switch (type)
        {
            case TransitionType.Blackout:
            case TransitionType.None:
                content.text = "";
                break;
            case TransitionType.ChangeToBoy:
                content.text = "密室（过去）";
                RoleController.Instance.ChangeRole(RoleType.MainRoleBoy);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Pass, StageType.BoxRoom, false);
                break;
            case TransitionType.ChangeToGirl:
                content.text = "藏间（现在）";
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                SceneController.Instance.ChangeScene(StageType.BoxRoom, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToBoy2:
                content.text = "密室（过去）";
                RoleController.Instance.ChangeRole(RoleType.MainRoleBoy);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Pass, StageType.SecretRoom_Now, false);
                break;
            case TransitionType.ChangeToGirl2:
                content.text = "密室（现在）";
                RoleController.Instance.ChangeRole(RoleType.MainRoleGirl);
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Now, StageType.SecretRoom_Pass, false);
                break;
            case TransitionType.ChangeToLibraryOut:
                content.text = "图书馆外";
                SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.SecretRoom_Now, false);
                break;
            case TransitionType.ChangeToSecretRoom_Now:
                content.text = "密室（现在）";
                SceneController.Instance.ChangeScene(StageType.SecretRoom_Now, StageType.LibraryOut, false);
                break;
        }
        Invoke("PlayTransitionOver", time);
    }

    private void PlayTransitionOver()
    {
        gameObject.SetActive(false);
    }
}

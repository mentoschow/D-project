using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    private ControllerManager api = ControllerManager.Get();


    public void GameStart()
    {
        Debug.Log("��Ϸ��ʼ��");
        api.UIController.homepageView?.gameObject.SetActive(false);
        api.sceneController.ChangeScene(Scene.Scene1);
    }

    
}

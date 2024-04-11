using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    private ControllerManager api = ControllerManager.Get();


    public void GameStart()
    {
        Debug.Log("游戏开始了");
        api.UIController.homepageView?.gameObject.SetActive(false);
        api.sceneController.ChangeScene(Scene.Scene1);
    }

    
}

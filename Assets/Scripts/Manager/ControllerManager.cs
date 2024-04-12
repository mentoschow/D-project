using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager
{
    public static ControllerManager Get()
    {
        if (_manager == null)
        {
            _manager = new ControllerManager();
        }
        return _manager;
    }
    private static ControllerManager _manager;  // µ¥Àý

    public GameController gameController;
    public UIController UIController;
    public ConfigController configController;
    public SceneController sceneController;

    public GameDataProxy gameData;

    public void init()
    {
        Debug.Log("init manager");
        gameController = new GameController();
        UIController = new UIController();
        configController = new ConfigController();
        sceneController = new SceneController();
        gameData = new GameDataProxy();

        //configController?.InitConfig();
    }
}

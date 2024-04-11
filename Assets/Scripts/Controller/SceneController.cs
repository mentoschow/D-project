using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController
{
    public Dictionary<Scene, GameObject> sceneMap = new Dictionary<Scene, GameObject>();

    void Start()
    {
        var config = ControllerManager.Get().configController;
        for (int i = 0; i < config.scenes.Count; i++)
        {
            sceneMap.Add(config.scenes[i], config.sceneGameObjs[i]);
        }
    }

    public void ChangeScene(Scene scene)
    {
        ControllerManager.Get().gameData.curScene = scene;
    }
}

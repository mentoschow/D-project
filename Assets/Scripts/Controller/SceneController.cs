using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : Singleton<SceneController>
{
    public Dictionary<Scene, GameObject> sceneMap = new Dictionary<Scene, GameObject>();

    void Start()
    {

    }

    public void ChangeScene(Scene scene)
    {
        
    }
}

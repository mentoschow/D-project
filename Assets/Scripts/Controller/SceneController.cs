using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneController : MonoSingleton<SceneController>
{
    public List<GameObject> sceneObj;
    public Dictionary<SceneType, GameObject> sceneMap = new Dictionary<SceneType, GameObject>();
    private SceneType curSceneID;

    public float sceneBgWidth;
    public float sceneBgHeight;

    void Start()
    {

    }

    private void CreateScene(SceneType sceneName)
    {
        foreach (var obj in sceneObj)
        {
            if (obj.name == sceneName.ToString())
            {
                var scene = Instantiate(obj);
                scene.transform.SetParent(transform);
                sceneMap[sceneName] = scene;
            }
        }
    }

    public void ChangeScene(SceneType scene)
    {
        curSceneID = scene;
        if (!sceneMap.ContainsKey(curSceneID))
        {
            CreateScene(scene);
        }

        Texture2D texture = sceneMap[curSceneID]?.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("sceneBgWidth:" + sceneBgWidth);
        Debug.Log("sceneBgHeight:" + sceneBgHeight);
    }
}

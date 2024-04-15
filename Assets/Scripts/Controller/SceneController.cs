using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneController : MonoSingleton<SceneController>
{
    public Dictionary<Scene, GameObject> sceneMap = new Dictionary<Scene, GameObject>();
    public GameObject scene;

    public float sceneBgWidth;
    public float sceneBgHeight;

    void Start()
    {
        
        foreach (int scene in Enum.GetValues(typeof(Scene)))
        {
            string sceneName = ((Scene)scene).ToString();
            //Debug.Log(sceneName);
        }

        var texture = scene.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("sceneBgWidth:" + sceneBgWidth);
        Debug.Log("sceneBgHeight:" + sceneBgHeight);
        ConfigController.Instance.GetChapterConfig(1);
    }

    public void ChangeScene(Scene scene)
    {
        
    }
}

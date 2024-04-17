using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneController : MonoSingleton<SceneController>
{
    public List<string> sceneID = new List<string>();
    public Dictionary<string, GameObject> sceneMap = new Dictionary<string, GameObject>();
    private string curSceneID;

    public float sceneBgWidth;
    public float sceneBgHeight;

    void Start()
    {
        foreach (string scene in sceneID)
        {
            GameObject sceneObj = transform.Find(scene)?.gameObject;
            if (sceneObj == null)
            {
                sceneMap.Add(scene, sceneObj);
            }
        }
    }

    public void ChangeScene(string scene)
    {
        curSceneID = scene;
        if (!sceneMap.ContainsKey(curSceneID))
        {
            Debug.LogError("切换场景失败：没有该场景ID" + curSceneID);
            return;
        }
        Texture2D texture = sceneMap[curSceneID]?.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("sceneBgWidth:" + sceneBgWidth);
        Debug.Log("sceneBgHeight:" + sceneBgHeight);
    }
}

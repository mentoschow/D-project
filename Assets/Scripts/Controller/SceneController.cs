using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneController : MonoSingleton<SceneController>
{
    public Dictionary<SceneType, GameObject> sceneMap = new Dictionary<SceneType, GameObject>();
    private SceneType curSceneID;

    public float sceneBgWidth;
    public float sceneBgHeight;

    void Start()
    {
        if (ResourcesController.Instance.sceneRes.Count > 0)
        {
            foreach (var scene in ResourcesController.Instance.sceneRes)
            {
                CreateScene(scene.Key, scene.Value.prefab);
            }
        }
    }

    private void CreateScene(SceneType sceneName, GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        var scene = Instantiate(obj);
        scene.transform.SetParent(transform);
        sceneMap[sceneName] = scene;
    }

    public void ChangeScene(SceneType scene)
    {
        curSceneID = scene;
        if (!sceneMap.ContainsKey(curSceneID))
        {
            Debug.LogError("�л�����ʧ��:" + curSceneID.ToString());
            return;
        }
        foreach (var s in sceneMap)
        {
            if (s.Key == curSceneID)
            {
                s.Value.SetActive(true);
            }
            else
            {
                s.Value.SetActive(false);
            }
        }
        Debug.Log("�л�����:" + curSceneID.ToString());
        Texture2D texture = sceneMap[curSceneID]?.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("�������:" + sceneBgWidth);
        Debug.Log("�����߶�:" + sceneBgHeight);
        // ���ý�ɫλ��
        if (ResourcesController.Instance.sceneRes[scene].bornPosX.Count > 0)
        {
            float posX = ResourcesController.Instance.sceneRes[scene].bornPosX[0];
            RoleController.Instance.SetRolePos(posX);
        }
        else
        {
            Debug.LogError("û�����ó�����:" + curSceneID.ToString());
        }
    }
}

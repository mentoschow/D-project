using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SceneController : MonoSingleton<SceneController>
{
    public Dictionary<StageType, GameObject> sceneMap = new Dictionary<StageType, GameObject>();
    private StageType curSceneID;
    private Dictionary<StageType, int> comeInSceneTimes = new Dictionary<StageType, int>();
    private Dictionary<string, EquipmentView> equipmentList = new Dictionary<string, EquipmentView>();

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
        foreach (var s in sceneMap)
        {
            if (s.Key == StageType.LibraryOut)
            {
                s.Value.SetActive(true);
            }
            else
            {
                s.Value.SetActive(false);
            }
        }
        Debug.Log("切换场景:" + StageType.LibraryOut.ToString());
        Texture2D texture = sceneMap[StageType.LibraryOut]?.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("场景宽度:" + sceneBgWidth);
        Debug.Log("场景高度:" + sceneBgHeight);
    }

    public void ResetData()
    {
        foreach (var scene in ResourcesController.Instance.sceneRes)
        {
            comeInSceneTimes[scene.Key] = 0;
        }
    }

    private void CreateScene(StageType sceneName, GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        var scene = Instantiate(obj);
        scene.transform.SetParent(transform);
        sceneMap[sceneName] = scene;
        comeInSceneTimes[sceneName] = 0;
        foreach (var child in scene.transform.Find("EquipmentLayer").GetComponentsInChildren<EquipmentView>())
        {
            if (child != null)
            {
                equipmentList[child.equipmentID] = child;
            }
        }
    }

    public void ChangeScene(StageType toScene, StageType fromScene, bool useTransition = true, bool setRolePos = true)
    {
        var res = ResourcesController.Instance.sceneRes[toScene];
        if (useTransition)
        {
            UIController.Instance.ShowTransition(res.name);
        }
        curSceneID = toScene;
        if (!sceneMap.ContainsKey(curSceneID))
        {
            Debug.LogError("切换场景失败:" + curSceneID.ToString());
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
        Debug.Log("切换场景:" + curSceneID.ToString());
        Texture2D texture = sceneMap[curSceneID]?.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("场景宽度:" + sceneBgWidth);
        Debug.Log("场景高度:" + sceneBgHeight);
        // 设置角色位置
        if (setRolePos)
        {
            float posX = 0;
            if (fromScene == StageType.None && toScene == StageType.LibraryOut)
            {
                // 游戏开始
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.LibraryOut && toScene == StageType.LibraryIn)
            {
                // 从图书馆外到图书馆内
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.LibraryIn && toScene == StageType.LibraryOut)
            {
                // 从图书馆内到图书馆外
                posX = res.rightPosX;
            }
            RoleController.Instance.SetRolePos(posX);
        }

        // 判断是否第一次进入
        if (comeInSceneTimes.ContainsKey(toScene))
        {
            if (comeInSceneTimes[toScene] == 0)
            {
                // 第一次进入
                GameLineNode node = new GameLineNode();
                node.type = GameNodeType.StageStart;
                node.ID = toScene.ToString();
                MessageManager.Instance.Send(MessageDefine.StageStart, new MessageData(node));
            }
            comeInSceneTimes[toScene]++;
        }
    }

    public void UpdateEquipment(string equipmentID, bool enable)
    {
        if (equipmentList.ContainsKey(equipmentID))
        {
            equipmentList[equipmentID].interactive = enable;
        }
    }
}

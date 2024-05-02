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
        Debug.Log("�л�����:" + StageType.LibraryOut.ToString());
        Texture2D texture = sceneMap[StageType.LibraryOut]?.transform.Find("bg").transform.GetComponent<SpriteRenderer>().sprite.texture;
        sceneBgWidth = texture.width;
        sceneBgHeight = texture.height;
        Debug.Log("�������:" + sceneBgWidth);
        Debug.Log("�����߶�:" + sceneBgHeight);
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
        if (setRolePos)
        {
            float posX = 0;
            if (fromScene == StageType.None && toScene == StageType.LibraryOut)
            {
                // ��Ϸ��ʼ
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.LibraryOut && toScene == StageType.LibraryIn)
            {
                // ��ͼ����⵽ͼ�����
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.LibraryIn && toScene == StageType.LibraryOut)
            {
                // ��ͼ����ڵ�ͼ�����
                posX = res.rightPosX;
            }
            RoleController.Instance.SetRolePos(posX);
        }

        // �ж��Ƿ��һ�ν���
        if (comeInSceneTimes.ContainsKey(toScene))
        {
            if (comeInSceneTimes[toScene] == 0)
            {
                // ��һ�ν���
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

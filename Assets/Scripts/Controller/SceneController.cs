using DG.Tweening;
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
    private Dictionary<DoorType, EquipmentView> doorList = new Dictionary<DoorType, EquipmentView>();

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
        var childList = scene.transform.Find("EquipmentLayer").GetComponentsInChildren<EquipmentView>();
        foreach (var child in childList)
        {
            if (child != null)
            {
                equipmentList[child.equipmentID] = child;
                if (child.doorType != DoorType.None)
                {
                    doorList[child.doorType] = child;
                }
                child.gameObject.SetActive(child.interactive);
            }
        }
    }

    public void ChangeScene(StageType toScene, StageType fromScene = StageType.None, bool useTransition = true, bool setRolePos = true)
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
            // ������ͼ�ƶ�
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
            else if (fromScene == StageType.LibraryIn && toScene == StageType.Passage)
            {
                // ��ͼ����ڵ�����
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.Passage && toScene == StageType.LibraryIn)
            {
                // �����ȵ�ͼ�����
                posX = res.rightPosX;
            }
            else if (fromScene == StageType.Passage && toScene == StageType.BoxRoom)
            {
                // �����ȵ��ؼ�
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.BoxRoom && toScene == StageType.Passage)
            {
                // �Ӳؼ䵽����
                posX = res.rightPosX;
            }
            else if (fromScene == StageType.BoxRoom && toScene == StageType.SecretRoom_Now)
            {
                // �Ӳؼ䵽��������
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.SecretRoom_Now && toScene == StageType.BoxRoom)
            {
                // ���������ڵ��ؼ�
                posX = res.rightPosX;
            }
            // ��Խ
            else if (fromScene == StageType.BoxRoom && toScene == StageType.SecretRoom_Pass)
            {
                // �Ӳؼ䵽���ҹ�ȥ
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.SecretRoom_Pass && toScene == StageType.BoxRoom)
            {
                // ���������ҵ��ؼ�
                posX = res.rightPosX;
            }
            else if (fromScene == StageType.SecretRoom_Now && toScene == StageType.SecretRoom_Pass)
            {
                // ���������ڵ����ҹ�ȥ
                posX = res.leftPosX;
            }
            else if (fromScene == StageType.SecretRoom_Pass && toScene == StageType.SecretRoom_Now)
            {
                // �����ҹ�ȥ����������
                posX = res.leftPosX;
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
            equipmentList[equipmentID].gameObject.SetActive(enable);
        }
    }

    public void UpdateDoor(DoorType type, bool enable)
    {
        if (doorList.ContainsKey(type))
        {
            doorList[type].interactive = enable;
            
        }
    }

    public void OnJewelryPuzzleDone()
    {
        // �ƿ�����
        if (equipmentList.ContainsKey("OrganClosetInBoxRoom"))
        {
            UpdateEquipment("BoxRoomRightDoorClose", true);
            UIController.Instance.HideGamePlayView();
            AudioController.Instance.PlayAudioEffect(AudioType.ClosetMove);
            var obj = equipmentList["OrganClosetInBoxRoom"].transform;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(obj.DOMoveX(7, 2)).AppendCallback(() =>
            {
                GameLineNode lineNode = new GameLineNode();
                lineNode.type = GameNodeType.Puzzle;
                lineNode.ID = PuzzleType.JewelryPuzzle.ToString();
                MessageManager.Instance.Send(MessageDefine.GameLineNodeDone, new MessageData(lineNode));
            });
        }
        else
        {
            Debug.LogError("���Ӳ����ڣ�");
        }
    }
}

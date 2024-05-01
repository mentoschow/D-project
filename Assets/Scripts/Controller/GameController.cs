using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public GameController() 
    {
        MessageManager.Instance.Register(MessageDefine.StageStart, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.PlayEpisodeDone, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.InteractWithEquipment, OnInteractWithEquipment);
        MessageManager.Instance.Register(MessageDefine.GetItemDone, CheckNextGameNode);
        var c = ConfigController.Instance;
    }

    public void CheckNextGameNode(MessageData node)
    {
        Debug.Log("����Ƿ����Զ�������һ��" + node.gameLineNode);
        if (node.gameLineNode == null)
        {
            Debug.LogError("�ڵ�����Ϊ��");
            return;
        }
        var nextNode = ConfigController.Instance.GetGameLineNode(node.gameLineNode);
        if (nextNode == null)
        {
            Debug.Log("û���Զ�����:" + node.gameLineNode.ID);
            return;
        }

        switch (nextNode.type)
        {
            case GameNodeType.GameEnd:
                Debug.Log("�Զ�������Ϸ������" + nextNode.ID);
                GameDataProxy.Instance.canOperate = false;
                UIController.Instance.GameEnd();
                break;
            case GameNodeType.NormalEpisode:
                Debug.Log("�Զ�������ͨ�Ի���" + nextNode.ID);
                GameDataProxy.Instance.canOperate = false;
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.PhoneEpisode:
                Debug.Log("�Զ������ֻ��Ի���" + nextNode.ID);
                GameDataProxy.Instance.canOperate = false;
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.Puzzle:
                GameDataProxy.Instance.canOperate = false;
                break;
            case GameNodeType.FreeOperate:
                Debug.Log("���ɲ���");
                GameDataProxy.Instance.canOperate = true;
                break;
        }
    }

    public void GameStart()
    {
        Debug.Log("��Ϸ��ʼ��");
        GameDataProxy.Instance.resetData();
        SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.None);
    }

    private void OnInteractWithEquipment(MessageData data)
    {
        string equipmentID = data.valueString;
        Debug.Log("���豸������" + equipmentID);
        if (string.IsNullOrEmpty(equipmentID))
        {
            return;
        }
        EquipmentConfig equipmentConfig = ConfigController.Instance.GetEquipmentConfig(equipmentID);
        if (equipmentConfig != null)
        {
            if (equipmentConfig.triggerEpisodeID != null)
            {
                // ��������
                UIController.Instance.PlayEpisode(equipmentConfig.triggerEpisodeID);
            }
            else if (equipmentConfig.triggerPuzzleID != null)
            {
                // ��������
            }
        }
    }
}

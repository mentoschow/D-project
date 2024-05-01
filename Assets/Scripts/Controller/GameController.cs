using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public GameController() 
    {
        MessageManager.Instance.Register(MessageDefine.PlayTransitionDone, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.GameStart, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.PlayEpisodeDone, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.InteractWithEquipment, OnInteractWithEquipment);
        var c = ConfigController.Instance;
    }

    public void CheckNextGameNode(MessageData node)
    {
        Debug.Log("����Ƿ����Զ�������һ��" + node.gameLineNode);
        GameDataProxy.Instance.canOperate = false;
        if (node.gameLineNode == null)
        {
            Debug.LogError("�ڵ�����Ϊ��");
            FreeOperate();
            return;
        }
        var nextNode = ConfigController.Instance.GetGameLineNode(node.gameLineNode);
        if (nextNode == null)
        {
            Debug.Log("û���Զ�����:" + node.gameLineNode.ID);
            FreeOperate();
            return;
        }

        switch (nextNode.type)
        {
            case GameNodeType.Transition:
                Debug.Log("�Զ�����ת����" + nextNode.ID);
                UIController.Instance.ShowTransition((TransitionType)Enum.Parse(typeof(TransitionType), nextNode.ID));
                break;
            case GameNodeType.GameEnd:
                Debug.Log("�Զ�������Ϸ������" + nextNode.ID);
                UIController.Instance.GameEnd();
                break;
            case GameNodeType.NormalEpisode:
                Debug.Log("�Զ�������ͨ�Ի���" + nextNode.ID);
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.PhoneEpisode:
                Debug.Log("�Զ������ֻ��Ի���" + nextNode.ID);
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.Puzzle:
                break;
        }
    }

    private void FreeOperate()
    {
        Debug.Log("�Զ�����");
        UIController.Instance.HideAllView();
        GameDataProxy.Instance.canOperate = true;
    }

    public void GameStart()
    {
        Debug.Log("��Ϸ��ʼ��");
        SceneController.Instance.ChangeScene(StageType.LibraryIn);
        GameLineNode node = new GameLineNode();
        node.type = GameNodeType.StageStart;
        node.ID = StageType.LibraryOut.ToString();
        MessageManager.Instance.Send(MessageDefine.GameStart, new MessageData(node));
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
            }
            else if (equipmentConfig.triggerPuzzleID != null)
            {
                // ��������
            }
        }
    }
}

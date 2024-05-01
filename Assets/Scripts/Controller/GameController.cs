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
        Debug.Log("检查是否能自动进行下一步" + node.gameLineNode);
        GameDataProxy.Instance.canOperate = false;
        if (node.gameLineNode == null)
        {
            Debug.LogError("节点数据为空");
            FreeOperate();
            return;
        }
        var nextNode = ConfigController.Instance.GetGameLineNode(node.gameLineNode);
        if (nextNode == null)
        {
            Debug.Log("没有自动触发:" + node.gameLineNode.ID);
            FreeOperate();
            return;
        }

        switch (nextNode.type)
        {
            case GameNodeType.Transition:
                Debug.Log("自动触发转场：" + nextNode.ID);
                UIController.Instance.ShowTransition((TransitionType)Enum.Parse(typeof(TransitionType), nextNode.ID));
                break;
            case GameNodeType.GameEnd:
                Debug.Log("自动触发游戏结束：" + nextNode.ID);
                UIController.Instance.GameEnd();
                break;
            case GameNodeType.NormalEpisode:
                Debug.Log("自动触发普通对话：" + nextNode.ID);
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.PhoneEpisode:
                Debug.Log("自动触发手机对话：" + nextNode.ID);
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.Puzzle:
                break;
        }
    }

    private void FreeOperate()
    {
        Debug.Log("自动操作");
        UIController.Instance.HideAllView();
        GameDataProxy.Instance.canOperate = true;
    }

    public void GameStart()
    {
        Debug.Log("游戏开始了");
        SceneController.Instance.ChangeScene(StageType.LibraryIn);
        GameLineNode node = new GameLineNode();
        node.type = GameNodeType.StageStart;
        node.ID = StageType.LibraryOut.ToString();
        MessageManager.Instance.Send(MessageDefine.GameStart, new MessageData(node));
    }

    private void OnInteractWithEquipment(MessageData data)
    {
        string equipmentID = data.valueString;
        Debug.Log("与设备交互：" + equipmentID);
        if (string.IsNullOrEmpty(equipmentID))
        {
            return;
        }
        EquipmentConfig equipmentConfig = ConfigController.Instance.GetEquipmentConfig(equipmentID);
        if (equipmentConfig != null)
        {
            if (equipmentConfig.triggerEpisodeID != null)
            {
                // 触发剧情
            }
            else if (equipmentConfig.triggerPuzzleID != null)
            {
                // 触发解谜
            }
        }
    }
}

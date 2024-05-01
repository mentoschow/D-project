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
        Debug.Log("检查是否能自动进行下一步" + node.gameLineNode);
        if (node.gameLineNode == null)
        {
            Debug.LogError("节点数据为空");
            return;
        }
        var nextNode = ConfigController.Instance.GetGameLineNode(node.gameLineNode);
        if (nextNode == null)
        {
            Debug.Log("没有自动触发:" + node.gameLineNode.ID);
            return;
        }

        switch (nextNode.type)
        {
            case GameNodeType.GameEnd:
                Debug.Log("自动触发游戏结束：" + nextNode.ID);
                GameDataProxy.Instance.canOperate = false;
                UIController.Instance.GameEnd();
                break;
            case GameNodeType.NormalEpisode:
                Debug.Log("自动触发普通对话：" + nextNode.ID);
                GameDataProxy.Instance.canOperate = false;
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.PhoneEpisode:
                Debug.Log("自动触发手机对话：" + nextNode.ID);
                GameDataProxy.Instance.canOperate = false;
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.Puzzle:
                GameDataProxy.Instance.canOperate = false;
                break;
            case GameNodeType.FreeOperate:
                Debug.Log("自由操作");
                GameDataProxy.Instance.canOperate = true;
                break;
        }
    }

    public void GameStart()
    {
        Debug.Log("游戏开始了");
        GameDataProxy.Instance.resetData();
        SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.None);
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
                UIController.Instance.PlayEpisode(equipmentConfig.triggerEpisodeID);
            }
            else if (equipmentConfig.triggerPuzzleID != null)
            {
                // 触发解谜
            }
        }
    }
}

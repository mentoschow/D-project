using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
        GameDataProxy.Instance.canOperate = false;
        switch (nextNode.type)
        {
            case GameNodeType.Transition:
                Debug.Log("自动触发转场：" + nextNode.ID);
                var transitionType = BaseFunction.ChangeStringToEnum<TransitionType>(nextNode.ID);
                UIController.Instance.ShowTransition(transitionType);
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
                Debug.Log("自动触发谜题：" + nextNode.ID);
                var config = ConfigController.Instance.GetMergeClueConfig(nextNode.ID);
                if (config != null)
                {
                    // 线索合并
                    bool canTrigger = true;
                    if (config.needFinishEpisodeID?.Count > 0)
                    {
                        foreach (var id in config.needFinishEpisodeID)
                        {
                            if (!GameDataProxy.Instance.finishedEpisode.Contains(id))
                            {
                                canTrigger = false;
                                break;
                            }
                        }
                    }
                    if (canTrigger)
                    {
                        UIController.Instance.showPuzzleView();
                    }
                }
                else
                {
                    var puzzleType = BaseFunction.ChangeStringToEnum<PuzzleType>(nextNode.ID);
                    if (puzzleType == PuzzleType.JewelryPuzzleDone)
                    {
                        UIController.Instance.showPuzzleView();
                    }
                    else if (puzzleType == PuzzleType.MimaPuzzleDone)
                    {
                        UIController.Instance.showMimaView();
                    }
                }
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
        UIController.Instance.HideAllView();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(RoleController.Instance.curRoleView.transform.DOMove(new Vector2(-5f, -0.466f), 3)).AppendCallback(() =>
        {
            SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.None, false, false);
        });
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

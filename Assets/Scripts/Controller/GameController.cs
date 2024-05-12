using DG.Tweening;
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
        MessageManager.Instance.Register(MessageDefine.GameLineNodeDone, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.InteractWithEquipment, OnInteractWithEquipment);
        var c = ConfigController.Instance;
    }

    public void CheckNextGameNode(MessageData node)
    {
        if (node.gameLineNode == null)
        {
            Debug.LogError("节点数据为空");
            return;
        }
        if (GameDataProxy.Instance.CheckIsGameLineNodeDone(node.gameLineNode))
        {
            Debug.LogError("自动流程已经触发过");
            return;
        }
        GameDataProxy.Instance.doneGameLineNode.Add(node.gameLineNode);
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
                GameEnd();
                break;
            case GameNodeType.Episode:
                Debug.Log("自动触发情节：" + nextNode.ID);
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.Puzzle:
                Debug.Log("自动触发谜题：" + nextNode.ID);
                TryOpenPuzzle(nextNode.ID);
                break;
            case GameNodeType.FreeOperate:
                Debug.Log("自由操作");
                GameDataProxy.Instance.canOperate = true;
                break;
            case GameNodeType.CharacterMove:
                Debug.Log("自动触发角色移动：" + nextNode.ID);
                UIController.Instance.HideGamePlayView();
                RoleController.Instance.PlayAutoMove(nextNode.ID);
                break;
        }
    }

    private void GameEnd()
    {
        UIController.Instance.GameEnd();
    }

    public void GameStart()
    {
        Debug.Log("游戏开始了");
        UIController.Instance.HideGamePlayView();
        UIController.Instance.ShowTransition("图书馆外");
        RoleController.Instance.curRoleView.transform.position = new Vector2(-13f, -0.216f);
        RoleController.Instance.curRoleView.moveVec = MoveVector.Right;
        AudioController.Instance.PlaySyncAudioEffect(AudioType.Clock);
        RoleController.Instance.curRoleView.transform.DOMove(new Vector2(-10f, -0.216f), 1).OnComplete(() =>
        {
            AudioController.Instance.PlayBgm(AudioType.NormalBgm);
            RoleController.Instance.curRoleView.moveVec = MoveVector.None;
            SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.None, false, false); // 特殊处理
        });
    }

    private void OnInteractWithEquipment(MessageData data)
    {
        string equipmentID = data.valueString;
        Debug.Log("与设备交互：" + equipmentID);
        if (equipmentID == null || equipmentID == "")
        {
            return;
        }
        EquipmentConfig equipmentConfig = ConfigController.Instance.GetEquipmentConfig(equipmentID);
        if (equipmentConfig != null)
        {
            if (GameDataProxy.Instance.equipmentInteractTimes.ContainsKey(equipmentID))
            {
                GameDataProxy.Instance.equipmentInteractTimes[equipmentID]++;
            }
            else
            {
                GameDataProxy.Instance.equipmentInteractTimes.Add(equipmentID, 1);
            }
            if (equipmentConfig.triggerPuzzleID != "")
            {
                // 触发解谜
                TryOpenPuzzle(equipmentConfig.triggerPuzzleID);
            }
            if (equipmentConfig.triggerEpisodeID != "")
            {
                // 触发剧情
                if (equipmentConfig.isTriggerEpisodeOnlyOnce && GameDataProxy.Instance.finishedEpisode.Contains(equipmentConfig.triggerEpisodeID))
                {
                    Debug.LogWarning("仅可触发一次，已经触发过");
                    return;
                }
                UIController.Instance.PlayEpisode(equipmentConfig.triggerEpisodeID);
            }
        }
    }

    private void TryOpenPuzzle(string ID)
    {
        GameDataProxy.Instance.canOperate = false;
        var config = ConfigController.Instance.GetMergeClueConfig(ID);
        if (config.ID != null)
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
            if (GameDataProxy.Instance.finishedClueCombine.Contains(ID))
            {
                canTrigger = false;
            }
            if (canTrigger)
            {
                UIController.Instance.showClueCombineView(config);
            }
            else
            {
                GameDataProxy.Instance.canOperate = true;
            }
        }
        else
        {
            var puzzleType = BaseFunction.ChangeStringToEnum<PuzzleType>(ID);
            if (puzzleType == PuzzleType.JewelryPuzzle)
            {
                UIController.Instance.showJiguanguiView();
            }
            else if (puzzleType == PuzzleType.PasswordPuzzle)
            {
                if (GameDataProxy.Instance.canPlayMima)
                {
                    UIController.Instance.showMimaView();
                }
                else
                {
                    GameDataProxy.Instance.canOperate = true;
                }
            }
            else
            {
                GameDataProxy.Instance.canOperate = true;
            }
        }
    }
}

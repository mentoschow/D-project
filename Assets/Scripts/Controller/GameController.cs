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
        GameDataProxy.Instance.canOperate = false;
        switch (nextNode.type)
        {
            case GameNodeType.Transition:
                Debug.Log("�Զ�����ת����" + nextNode.ID);
                var transitionType = BaseFunction.ChangeStringToEnum<TransitionType>(nextNode.ID);
                UIController.Instance.ShowTransition(transitionType);
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
                Debug.Log("�Զ��������⣺" + nextNode.ID);
                var config = ConfigController.Instance.GetMergeClueConfig(nextNode.ID);
                if (config != null)
                {
                    // �����ϲ�
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
                Debug.Log("���ɲ���");
                GameDataProxy.Instance.canOperate = true;
                break;
        }
    }

    public void GameStart()
    {
        Debug.Log("��Ϸ��ʼ��");
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

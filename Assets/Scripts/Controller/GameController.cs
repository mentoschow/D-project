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
        MessageManager.Instance.Register(MessageDefine.GameLineNodeDone, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.InteractWithEquipment, OnInteractWithEquipment);
        var c = ConfigController.Instance;
    }

    public void CheckNextGameNode(MessageData node)
    {
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
            case GameNodeType.Episode:
                Debug.Log("�Զ�������ڣ�" + nextNode.ID);
                UIController.Instance.PlayEpisode(nextNode.ID);
                break;
            case GameNodeType.Puzzle:
                Debug.Log("�Զ��������⣺" + nextNode.ID);
                TryOpenPuzzle(nextNode.ID);
                break;
            case GameNodeType.FreeOperate:
                Debug.Log("���ɲ���");
                GameDataProxy.Instance.canOperate = true;
                break;
            case GameNodeType.CharacterMove:
                Debug.Log("�Զ�������ɫ�ƶ���" + nextNode.ID);
                UIController.Instance.HideGamePlayView();
                RoleController.Instance.PlayAutoMove(nextNode.ID);
                break;
        }
    }

    public void GameStart()
    {
        Debug.Log("��Ϸ��ʼ��");
        UIController.Instance.HideGamePlayView();
        RoleController.Instance.curRoleView.transform.position = new Vector2(-13f, -0.216f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(RoleController.Instance.curRoleView.transform.DOMove(new Vector2(-5f, -0.216f), 3)).AppendCallback(() =>
        {
            SceneController.Instance.ChangeScene(StageType.LibraryOut, StageType.None, false, false); // ���⴦��
        });
    }

    private void OnInteractWithEquipment(MessageData data)
    {
        string equipmentID = data.valueString;
        Debug.Log("���豸������" + equipmentID);
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
                // ��������
                TryOpenPuzzle(equipmentConfig.triggerPuzzleID);
            }
            if (equipmentConfig.triggerEpisodeID != "")
            {
                // ��������
                UIController.Instance.PlayEpisode(equipmentConfig.triggerEpisodeID);
            }
        }
    }

    private void TryOpenPuzzle(string ID)
    {
        var config = ConfigController.Instance.GetMergeClueConfig(ID);
        if (config.ID != null)
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
            if (puzzleType == PuzzleType.JewelryPuzzleDone)
            {
                UIController.Instance.showJiguanguiView();
            }
            else if (puzzleType == PuzzleType.MimaPuzzleDone)
            {
                UIController.Instance.showMimaView();
            }
            else
            {
                GameDataProxy.Instance.canOperate = true;
            }
        }
    }
}

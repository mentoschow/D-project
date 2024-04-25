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
        var c = ConfigController.Instance;
    }

    public void CheckNextGameNode(MessageData node)
    {
        Debug.Log("检查是否能自动进行下一步" + node.gameLineNode);
        GameDataProxy.Instance.canOperate = false;
        if (node.gameLineNode == null)
        {
            return;
        }
        switch (node.gameLineNode.type)
        {
            case GameNodeType.Stage1Start:
                UIController.Instance.ShowTransition(TransitionType.GameStart);
                SceneController.Instance.ChangeScene(SceneType.LibraryOut);
                break;
            case GameNodeType.Transition:
                if (node.gameLineNode.ID == TransitionType.GameStart.ToString())
                {
                    UIController.Instance.PlayEpisode("TEST01_010_020");
                }
                break;
            case GameNodeType.GameEnd:
                UIController.Instance.GameEnd();
                break;
            case GameNodeType.Tutorial:
                break;
            case GameNodeType.NormalEpisode:
                switch (node.gameLineNode.ID)
                {
                    case "TEST01_010_010":
                        UIController.Instance.PlayEpisode("TEST01_010_020");
                        break;
                    case "TEST01_010_020":
                        FreeOperate();
                        break;
                    case "MS01_010_010":
                        UIController.Instance.PlayEpisode("MS01_010_020");
                        break;
                    case "MS01_010_020":
                        UIController.Instance.ShowTutorial();
                        break;
                }
                break;
            case GameNodeType.PhoneEpisode:
                break;
            case GameNodeType.Puzzle:
                break;
        }
    }

    private void FreeOperate()
    {
        UIController.Instance.HideAllView();
        GameDataProxy.Instance.canOperate = true;
    }

    public void GameStart()
    {
        Debug.Log("游戏开始了");
        GameLineNode node = new GameLineNode();
        node.type = GameNodeType.Stage1Start;
        MessageManager.Instance.Send(MessageDefine.GameStart, new MessageData(node));
        GameDataProxy.Instance.canOperate = true;
    }
}

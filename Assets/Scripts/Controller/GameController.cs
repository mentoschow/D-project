using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public GameController() 
    {
        MessageManager.Instance.Register(MessageDefine.ChangeSceneDone, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.GameStart, CheckNextGameNode);
        MessageManager.Instance.Register(MessageDefine.PlayEpisodeDone, CheckNextGameNode);
    }

    public void CheckNextGameNode(MessageData node)
    {
        Debug.Log("检查是否能自动进行下一步" + node.gameLineNode);
        GameDataProxy.Instance.canMainRoleMove = false;
        if (node.gameLineNode == null)
        {
            return;
        }
        switch (node.gameLineNode.type)
        {
            case GameNodeType.GameStart:
                UIController.Instance.ShowTransition(TransitionType.GameStart);
                SceneController.Instance.ChangeScene(SceneType.LibraryOut);
                break;
            case GameNodeType.Transition:
                if (node.gameLineNode.ID == SceneType.LibraryOut.ToString())
                {
                    //UIController.Instance.PlayEpisode("MS01_010_010");
                }
                else if (node.gameLineNode.ID == SceneType.LibraryIn.ToString())
                {

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
            case GameNodeType.FreeOperate:
                UIController.Instance.ShowScene();
                GameDataProxy.Instance.canMainRoleMove = true;
                break;
        }
    }

    public void GameStart()
    {
        Debug.Log("游戏开始了");
        GameLineNode node = new GameLineNode();
        node.type = GameNodeType.GameStart;
        MessageManager.Instance.Send(MessageDefine.GameStart, new MessageData(node));
        GameDataProxy.Instance.canMainRoleMove = true;
    }
}

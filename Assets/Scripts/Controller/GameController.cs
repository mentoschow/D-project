using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private Queue<GameLineConfig> gameLineConfig = new Queue<GameLineConfig>();

    public GameController() 
    {
        foreach (GameLineConfig config in ConfigController.Instance.gameLineConfigs)
        {
            gameLineConfig.Enqueue(config);
        }
    }

    public void CheckNextGameNode()
    {
        GameDataProxy.Instance.canMainRoleMove = false;
        GameLineConfig nextNodeConfig = gameLineConfig.Dequeue();
        if (nextNodeConfig == null)
        {
            return;
        }
        switch (nextNodeConfig.type)
        {
            case GameNodeType.GameEnd:

                break;
            case GameNodeType.Transition:
                UIController.Instance.ShowTransition(nextNodeConfig.ID);
                break;
            case GameNodeType.ChangeScene:
                SceneController.Instance.ChangeScene(nextNodeConfig.ID);
                break;
            case GameNodeType.Tutorial:
                break;
            case GameNodeType.NormalEpisode:
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
        //CheckNextGameNode();
        UIController.Instance.GameStart();
        GameDataProxy.Instance.canMainRoleMove = true;
    }
}

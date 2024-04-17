using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    private GameNodeType curGameNode;


    public void CheckNextGameNode()
    {

    }

    public void GameStart()
    {
        Debug.Log("游戏开始了");
        curGameNode = GameNodeType.GameStart;
        CheckNextGameNode();
        UIController.Instance.GameStart();
    }
}

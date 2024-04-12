using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{


    public void GameStart()
    {
        Debug.Log("游戏开始了");
        MessageManager.Instance.Send(MessageDefine.GameStart);
    }
}

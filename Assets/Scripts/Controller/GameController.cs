using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{


    public void GameStart()
    {
        Debug.Log("��Ϸ��ʼ��");
        MessageManager.Instance.Send(MessageDefine.GameStart);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    public void GameStart()
    {
        UIController UIController = ControllerManager.GetManager().UIController;
        UIController?.ChangeStage();
    }
}

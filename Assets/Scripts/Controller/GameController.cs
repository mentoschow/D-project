using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Start()
    {
        
    }

    public void GenerateGameConfig()
    {

    }

    public void GameStart()
    {
        UIController UIController = ControllerManager.GetManager().UIController;
        UIController?.ChangeStage();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    void Start()
    {
        ControllerManager.Get().init();
        ControllerManager.Get().UIController.Init(gameObject);
    }
}

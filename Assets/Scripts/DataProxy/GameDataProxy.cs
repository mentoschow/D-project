using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataProxy : Singleton<GameDataProxy>
{
    public List<DialogConfig> historyDialog = new List<DialogConfig>();
    public List<ItemConfig> bagItem = new List<ItemConfig>();
    public bool canOperate = false;
    public string doingTutorial = "";

    public GameDataProxy()
    {
        historyDialog = new List<DialogConfig>();
        bagItem = new List<ItemConfig>();
        canOperate = false;
    }
}

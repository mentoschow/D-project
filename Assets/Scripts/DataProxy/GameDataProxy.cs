using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataProxy : Singleton<GameDataProxy>
{
    public List<DialogConfig> historyDialog = new List<DialogConfig>();
    public List<ItemConfig> bagItem = new List<ItemConfig>();
    public bool canOperate = false;
    public string doingTutorial = "";

    public Dictionary<JewelryType,bool> jewelryCmpletion = new Dictionary<JewelryType, bool>();
    public List<PuzzleCombineConfig> puzzleCombineConfigs = new List<PuzzleCombineConfig>();
    public GameDataProxy()
    {
        historyDialog = new List<DialogConfig>();
        bagItem = new List<ItemConfig>();
        canOperate = false;
    }

    public bool checkJewelryComplete(JewelryType type)
    {
        bool complete = false;
        this.jewelryCmpletion.TryGetValue(type, out complete);
        return complete;
    }
    public bool checkJewelryCanUse(int code,JewelryType type)
    {
        bool result = false;
        JewelryType checkType = JewelryType.Not;
        foreach (KeyValuePair<JewelryType, bool> item in jewelryCmpletion)
        {
            if (!item.Value)
            {
                checkType = item.Key;

                break;
            }
        }
        if(checkType > 0 && checkType  == type)
        {
            PuzzleCombineConfig config = this.getCombineConfig(checkType);
            result = config.code == code;
        }
        return result;
    }
    public PuzzleCombineConfig getCombineConfig(JewelryType type)
    {
        PuzzleCombineConfig combineConfig=null;
        if (puzzleCombineConfigs.Count > 0)
        {
            combineConfig =  puzzleCombineConfigs.Where(item =>item.jewelryType == type ).FirstOrDefault();
        }

        return combineConfig;
    }
}

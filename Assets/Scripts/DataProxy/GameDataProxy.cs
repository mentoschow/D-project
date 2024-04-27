using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataProxy : Singleton<GameDataProxy>
{
    public List<DialogConfig> normalHistoryDialog = new List<DialogConfig>();
    public Dictionary<BelongPhoneGroup, List<DialogConfig>> phoneHistoryDialog = new Dictionary<BelongPhoneGroup, List<DialogConfig>>();
    public List<ItemConfig> bagItem = new List<ItemConfig>();
    public bool canOperate = false;
    public string doingTutorial = "";

    //public Dictionary<JewelryType,bool> jewelryCmpletion = new Dictionary<JewelryType, bool>();
    public Dictionary<JewelryType,int> insertjewelryMap = new Dictionary<JewelryType, int>();
    public Dictionary<JewelryType, int> rightInsertMap = new Dictionary<JewelryType, int>();

    public List<PuzzleCombineConfig> puzzleCombineConfigs = new List<PuzzleCombineConfig>();
    public GameDataProxy()
    {
        normalHistoryDialog = new List<DialogConfig>();
        bagItem = new List<ItemConfig>();
        canOperate = false;
    }

    public bool checkJewelryComplete(JewelryType type)
    {
        int useCode = 0;
        bool isExist = insertjewelryMap.TryGetValue(type, out useCode);
        bool isComplete = isExist && (useCode > 0);
        return isComplete;
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

    public bool checkInsertOver()
    {
        bool result = false;
        if(insertjewelryMap.Count == rightInsertMap.Count)
        {
            result = insertjewelryMap.SequenceEqual(rightInsertMap);
            if (!result)
            {
                insertjewelryMap.Clear();
            }
        }
        return result;
    }
}

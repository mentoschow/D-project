using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataProxy : Singleton<GameDataProxy>
{
    public List<DialogConfig> normalHistoryDialog = new List<DialogConfig>();
    public Dictionary<BelongPhoneGroup, List<DialogConfig>> phoneHistoryDialog = new Dictionary<BelongPhoneGroup, List<DialogConfig>>();
    public List<string> mainGirlBagItem = new List<string>();
    public List<string> mainBoyBagItem = new List<string>();
    public List<string> finishedEpisode = new List<string>();  // 已经结束的剧情
    public Dictionary<string, int> equipmentInteractTimes = new Dictionary<string, int>();
    public bool canOperate = false;

    //public Dictionary<JewelryType,bool> jewelryCmpletion = new Dictionary<JewelryType, bool>();
    public Dictionary<JewelryType,int> insertjewelryMap = new Dictionary<JewelryType, int>();
    public Dictionary<JewelryType, int> rightInsertMap = new Dictionary<JewelryType, int>();

    public List<int> rightMimaList = new List<int> { 5, 4, 3, 3, 1 };
    public List<int> useMimaList = new List<int> ();

    public List<PuzzleCombineConfig> puzzleCombineConfigs = new List<PuzzleCombineConfig>();
    public GameDataProxy()
    {
        ResetData();
    }

    public void ResetData()
    {
        normalHistoryDialog = new List<DialogConfig>();
        phoneHistoryDialog = new Dictionary<BelongPhoneGroup, List<DialogConfig>>();
        mainGirlBagItem = new List<string>();
        mainBoyBagItem = new List<string>();
        finishedEpisode = new List<string>();
        equipmentInteractTimes = new Dictionary<string, int>();
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

    public bool checkMimaOver()
    {
        bool result = false;
        bool isEqual = useMimaList.SequenceEqual(rightMimaList);
        result = isEqual;

        return result;
    }

    public bool CheckHasClueItem(string ID, RoleType roleType)
    {
        if (roleType == RoleType.MainRoleGirl)
        {
            return mainGirlBagItem.Contains(ID);
        }
        else if (roleType == RoleType.MainRoleBoy)
        {
            return mainBoyBagItem.Contains(ID);
        }
        else
        {
            return true;
        }
    }
}

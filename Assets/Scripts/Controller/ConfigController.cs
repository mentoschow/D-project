using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum JewelryType
{
    Not = 0,
    Earring = 1,
    Necklace = 2,
    Bracelet = 3,
    Ring = 4,
    Watch = 5,
    Human = 6,
}


[Serializable]
public class PuzzleConfig
{
    public string ID;
    public PuzzleType type;
    public List<PuzzleItemConfig> itemConfigList;
}
[Serializable]
public class PuzzleItemConfig
{
    public string itemID;
    public JewelryType jewelryType;
    public List<PuzzleCombineConfig> combineList;
}
[Serializable]
public class PuzzleCombineConfig
{
    public float xPos;
    public float yPos;
    public int code;
    public JewelryType jewelryType;
}

public class ConfigController : Singleton<ConfigController>
{
    private const string configPath = "Assets/Resources/Configs/";
    private const string fileTailPath = ".csv";
    private Dictionary<string, string> dataFilePathDic = new Dictionary<string, string>() {
        {"GameLineConfig", configPath + "�����ĵ�-�Զ���������" + fileTailPath},
        {"ChapterConfig", configPath + "�����ĵ�-�½�" + fileTailPath},
        {"EpisodeConfig", configPath + "�����ĵ�-���" + fileTailPath},
        {"DialogConfig", configPath + "�����ĵ�-�Ի�" + fileTailPath},
        {"EquipmentConfig", configPath + "�����ĵ�-�豸" + fileTailPath},
        {"ItemConfig", configPath + "�����ĵ�-����" + fileTailPath},
        //{"CharacterAutoMoveConfig", configPath + "�����ĵ�-��ɫ�ƶ�" + fileTailPath},
    };

    private Dictionary<string, DataTable> datatableDic = new Dictionary<string, DataTable>();
    private Dictionary<GameLineNode, GameLineNode> gameLineConfig = new Dictionary<GameLineNode, GameLineNode>();  // �Զ�������������
    private Dictionary<string, ChapterConfig> chapterConfigList = new Dictionary<string, ChapterConfig>();  // �½�����
    private Dictionary<string, EpisodeConfig> episodeConfigList = new Dictionary<string, EpisodeConfig>();  // �Ի��������
    private Dictionary<string, DialogConfig> dialogConfigList = new Dictionary<string, DialogConfig>();  // �Ի�����
    private Dictionary<string, ChoiceConfig> choiceConfigList = new Dictionary<string, ChoiceConfig>();  // ѡ������
    private Dictionary<string, EquipmentConfig> equipmentConfigList = new Dictionary<string, EquipmentConfig>();  // �����豸����Ʒ������
    private Dictionary<string, ItemConfig> itemConfigList = new Dictionary<string, ItemConfig>();  // ���ߣ�����������
    private Dictionary<string, MergeClueConfig> mergeClueConfigList = new Dictionary<string, MergeClueConfig>();
    private Dictionary<string, CharacterAutoMoveConfig> characterAutoMoveConfigList = new Dictionary<string, CharacterAutoMoveConfig>();

    private string testJsonPath = configPath + "puzzle.json";
    public TextAsset jsonTextAsset; 
    public PuzzleConfig puzzleConfig;
    public ConfigController()
    {
        GenerateConfig();

        this.initPuzzleConfig();
        //PuzzleConfig ggboy = JsonUtility.FromJson(PuzzleConfig)("");
    }

    void initPuzzleConfig()
    {
        string json = File.ReadAllText(testJsonPath);
        this.puzzleConfig = JsonUtility.FromJson<PuzzleConfig>(json);

        List<PuzzleItemConfig> configList = this.puzzleConfig?.itemConfigList ?? new List<PuzzleItemConfig>();
        int len = configList?.Count ?? 0;
        if (len > 0)
        {
            foreach (PuzzleItemConfig itemConfig in configList)
            {
               List<PuzzleCombineConfig> list = itemConfig?.combineList ?? new List<PuzzleCombineConfig>();
                if (list.Count > 0)
                {
                    foreach(PuzzleCombineConfig combineConfig in list)
                    {
                        GameDataProxy.Instance.rightInsertMap.Add(combineConfig.jewelryType,combineConfig.code);
                    }
                    GameDataProxy.Instance.puzzleCombineConfigs = list.ToList();
                }
            }
        }
    }

    public GameLineNode GetGameLineNode(GameLineNode node)
    {
        foreach (var config in gameLineConfig)
        {
            if (config.Key.type == node.type && config.Key.ID == node.ID)
            {
                return config.Value;
            }
        }
        return null;
    }

    public ChapterConfig GetChapterConfig(string chapterID)
    {
        if (!chapterConfigList.ContainsKey(chapterID))
        {
            ChapterConfig config = new ChapterConfig();
            if (!datatableDic.ContainsKey("ChapterConfig")) {
                Debug.LogError("û���½����ñ�");
                return null;
            }
            var dt = datatableDic["ChapterConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("�½����ò����ڣ�id��" + chapterID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"].ToString() == chapterID.ToString())
                {
                    config.ID = chapterID;
                    config.name = row["name"].ToString();
                    chapterConfigList[chapterID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return chapterConfigList[chapterID];
        }
    }

    public EpisodeConfig GetEpisodeConfig(string episodeID)
    {
        if (!episodeConfigList.ContainsKey(episodeID))
        {
            EpisodeConfig config = new EpisodeConfig();
            if (!datatableDic.ContainsKey("EpisodeConfig"))
            {
                Debug.LogError("û��������ñ�");
                return null;
            }
            var dt = datatableDic["EpisodeConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("������ò����ڣ�id��" + episodeID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"].ToString() == episodeID.ToString())
                {
                    config.ID = episodeID;
                    config.episodeType = (EpisodeType)int.Parse(row["episodeType"].ToString());
                    string dialogList = row["dialogList"].ToString();
                    config.dialogList = new List<string>(dialogList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string enableEquipmentID = row["enableEquipmentID"].ToString();
                    config.enableEquipmentID = new List<string>(enableEquipmentID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string disableEquipmentID = row["disableEquipmentID"].ToString();
                    config.disableEquipmentID = new List<string>(disableEquipmentID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string needFinishEpisodeID = row["needFinishEpisodeID"].ToString();
                    config.needFinishEpisodeID = new List<string>(needFinishEpisodeID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string needItemID = row["needItemID"].ToString();
                    config.needItemID = new List<string>(needItemID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    config.isNeedRecord = int.Parse(row["isNeedRecord"].ToString()) == 1 ? true : false;
                    config.belongGroup = (BelongPhoneGroup)int.Parse(row["belongGroup"].ToString());

                    episodeConfigList[episodeID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return episodeConfigList[episodeID];
        }
    }

    public DialogConfig GetDialogConfig(string dialogID)
    {
        if (!dialogConfigList.ContainsKey(dialogID))
        {
            DialogConfig config = new DialogConfig();
            if (!datatableDic.ContainsKey("DialogConfig"))
            {
                Debug.LogError("û�жԻ����ñ�");
                return null;
            }
            var dt = datatableDic["DialogConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("�Ի����ò����ڣ�id��" + dialogID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"].ToString() == dialogID.ToString())
                {
                    config.ID = dialogID;
                    config.content = row["content"].ToString();
                    if (row["character"].ToString() != "")
                    {
                        config.roleType = (RoleType)int.Parse(row["character"].ToString());
                    }
                    else
                    {
                        config.roleType = RoleType.None;
                    }
                    string getItemID = row["getItemID"].ToString();
                    config.getItemID = new List<string>(getItemID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string choices = row["choices"].ToString();
                    config.choices = new List<string>(choices.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    dialogConfigList[dialogID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return dialogConfigList[dialogID];
        }
    }

    public ChoiceConfig GetChoiceConfig(string choiceID)
    {
        if (!choiceConfigList.ContainsKey(choiceID))
        {
            ChoiceConfig config = new ChoiceConfig();
            if (!datatableDic.ContainsKey("ChoiceConfig"))
            {
                Debug.LogError("û��ѡ�����ñ�");
                return null;
            }
            var dt = datatableDic["ChoiceConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("ѡ�����ò����ڣ�id��" + choiceID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"].ToString() == choiceID.ToString())
                {
                    config.ID = choiceID;
                    config.content = row["content"].ToString();
                    config.triggerEpisodeID = row["triggerEpisodeID"].ToString();

                    choiceConfigList[choiceID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return choiceConfigList[choiceID];
        }
    }

    public EquipmentConfig GetEquipmentConfig(string equipmentID)
    {
        if (!equipmentConfigList.ContainsKey(equipmentID))
        {
            EquipmentConfig config = new EquipmentConfig();
            if (!datatableDic.ContainsKey("EquipmentConfig"))
            {
                Debug.LogError("û���豸���ñ�");
                return null;
            }
            var dt = datatableDic["EquipmentConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("�豸���ò����ڣ�id��" + equipmentID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"].ToString() == equipmentID.ToString())
                {
                    config.ID = equipmentID;
                    config.name = row["name"].ToString();
                    config.description = row["description"].ToString();
                    config.triggerEpisodeID = row["triggerEpisodeID"].ToString();
                    config.triggerPuzzleID = row["triggerPuzzleID"].ToString();
                    string getItemID = row["getItemID"].ToString();
                    config.getItemID = new List<string>(getItemID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string mustDoneEpisodeID = row["mustDoneEpisodeID"].ToString();
                    config.mustDoneEpisodeID = new List<string>(mustDoneEpisodeID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    equipmentConfigList[equipmentID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return equipmentConfigList[equipmentID];
        }
    }

    public ItemConfig GetClueItemConfig(string itemID)
    {
        if (!itemConfigList.ContainsKey(itemID))
        {
            ItemConfig config = new ItemConfig();
            if (!datatableDic.ContainsKey("ItemConfig"))
            {
                Debug.LogError("û�е������ñ�");
                return null;
            }
            var dt = datatableDic["ItemConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("�������ò����ڣ�id��" + itemID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"]?.ToString() == itemID.ToString())
                {
                    config.ID = itemID;
                    config.name = row["name"]?.ToString();
                    config.description = row["description"]?.ToString();
                    config.isSaveBag = int.Parse(row["isSaveBag"]?.ToString()) == 1 ? true : false;

                    itemConfigList[itemID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return itemConfigList[itemID];
        }
    }

    public MergeClueConfig GetMergeClueConfig(string ID)
    {
        if (!mergeClueConfigList.ContainsKey(ID))
        {
            MergeClueConfig config = new MergeClueConfig();
            if (!datatableDic.ContainsKey("MergeClueConfig"))
            {
                Debug.LogError("û�������ϲ����ñ�");
                return null;
            }
            var dt = datatableDic["MergeClueConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("�����ϲ����ò����ڣ�id��" + ID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"]?.ToString() == ID.ToString())
                {
                    config.ID = ID;
                    string needFinishEpisodeID = row["needFinishEpisodeID"].ToString();
                    config.needFinishEpisodeID = new List<string>(needFinishEpisodeID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string prepareClueList = row["prepareClueList"].ToString();
                    config.prepareClueList = new List<string>(prepareClueList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    string correctClueList = row["correctClueList"].ToString();
                    config.correctClueList = new List<string>(correctClueList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    config.completeClue = row["completeClue"].ToString();

                    mergeClueConfigList[ID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return mergeClueConfigList[ID];
        }
    }

    public CharacterAutoMoveConfig GetCharacterAutoMoveConfig(string ID)
    {
        if (!characterAutoMoveConfigList.ContainsKey(ID))
        {
            CharacterAutoMoveConfig config = new CharacterAutoMoveConfig();
            if (!datatableDic.ContainsKey("CharacterAutoMoveConfig"))
            {
                Debug.LogError("û�н�ɫ�ƶ����ñ�");
                return null;
            }
            var dt = datatableDic["CharacterAutoMoveConfig"];
            if (dt.Rows.Count == 0)
            {
                Debug.LogError("��ɫ�ƶ����ò����ڣ�id��" + ID);
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                if (row["ID"]?.ToString() == ID.ToString())
                {
                    config.ID = ID;
                    config.posX = float.Parse(row["posX"].ToString());
                    config.posY = float.Parse(row["posY"].ToString());
                    config.duration = float.Parse(row["duration"].ToString());

                    characterAutoMoveConfigList[ID] = config;
                    break;
                }
            }
            return config;
        }
        else
        {
            return characterAutoMoveConfigList[ID];
        }
    }

    private void GenerateConfig()
    {
        foreach (var item in dataFilePathDic)
        {
            string filePath = item.Value;
            DataTable dt = new DataTable();
            try
            {
                //�ļ�����ȡ
                System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
                System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.GetEncoding("gb2312"));
                string tempText = "";
                bool isFirst = true;
                while ((tempText = sr.ReadLine()) != null)
                {
                    //һ���һ��Ϊ����
                    if (isFirst)
                    {
                        string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string str in arr)
                        {
                            dt.Columns.Add(str);
                        }
                        isFirst = false;
                    }
                    else
                    {
                        string[] arr = FromCsvLine(tempText);
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            string valueName = dt.Columns[i].ToString();
                            dr[valueName] = i < arr.Length ? arr[i] : "";
                        }
                        dt.Rows.Add(dr);
                    }
                }
                datatableDic[item.Key] = dt;
                //�ر���
                sr.Close();
                fs.Close();
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }
        GenerateGameLineConfig();
    }

    private void GenerateGameLineConfig()
    {
        if (!datatableDic.ContainsKey("GameLineConfig"))
        {
            Debug.LogError("û���Զ��������ñ�");
            return;
        }
        var dt = datatableDic["GameLineConfig"];
        if (dt.Rows.Count == 0)
        {
            Debug.LogError("�Զ�����������");
            return;
        }
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var row = dt.Rows[i];
            GameLineNode finishNode = new GameLineNode();
            GameLineNode triggerNode = new GameLineNode();
            finishNode.type = (GameNodeType)int.Parse(row["finishType"].ToString());
            finishNode.ID = row["finishID"].ToString();
            triggerNode.type = (GameNodeType)int.Parse(row["triggerType"].ToString());
            triggerNode.ID = row["triggerID"]?.ToString();
            gameLineConfig.Add(finishNode, triggerNode);
        }
    }

    /// <summary>
    /// ����һ��CSV����
    /// </summary>
    /// <param name="csv">csv������</param>
    /// <returns></returns>
    public static string[] FromCsvLine(string csv)
    {
        List<string> result = new List<string>();

        if (!string.IsNullOrEmpty(csv))
        {
            int startIndex = 0;
            for (int endIndex = 0; endIndex < csv.Length; endIndex++)
            {
                if (csv[endIndex] == '"')
                {
                    startIndex = endIndex + 1;
                    endIndex = startIndex;
                    while (csv[endIndex] != '"')
                    {
                        endIndex++;
                    }
                    result.Add(csv.Substring(startIndex, endIndex - startIndex));
                    startIndex = endIndex + 1;
                    endIndex = startIndex;
                }
                else if (csv[endIndex] == ',')
                {
                    result.Add(csv.Substring(startIndex, endIndex - startIndex));
                    startIndex = endIndex + 1;
                }
                else if (endIndex == csv.Length - 1)
                {
                    result.Add(csv.Substring(startIndex, endIndex - startIndex + 1));
                    startIndex = endIndex + 1;
                }
            }
        }
        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].Length == 0)
            {
                continue;
            }
            if (result[i][0] == ',')
            {
                result[i] = result[i].Substring(1, result[i].Length - 1);
            }
        }
        return result.ToArray();
    }

    public string getJewelryUrl(JewelryType type,int code)
    {
        string url = "Images/UI/Puzzle/";
        string itemName = Enum.GetName(typeof(JewelryType), type);
        url = url + itemName +"_"+ code.ToString();

        return url;
    }
}

public class GameLineNode
{
    public string ID;
    public GameNodeType type;
}

public enum GameNodeType
{
    GameEnd,  // ��Ϸ����
    StageStart,  // Ļ��ʼ
    Transition,  // ת��
    NormalEpisode,  // ��ͨ�Ի�
    PhoneEpisode,  // �ֻ��Ի�
    Puzzle,  // ����
    GotClueItem,  // �������
    FreeOperate,  // ���ɲ���
    CharacterMove,  // ��ɫ�ƶ�
}

public enum StageType
{
    None,
    LibraryOut,
    LibraryIn,
    Passage,
    BoxRoom,
    SecretRoom_Now,
    SecretRoom_Pass
}

public enum DoorType
{
    None,
    LibraryOut,
    LibraryInLeft,
    LibraryInRight,
}

public enum TransitionType
{
    Blackout,  // ͣ��
    ChangeToBoy,
    ChangeToGirl,
}

public class GameLineConfig
{
    public GameLineNode finishNode;
    public GameLineNode triggerNode;
}

public class ChapterConfig
{
    public string ID;
    public string name;
}

public class EpisodeConfig
{
    public string ID;
    public List<string> dialogList;
    public List<string> enableEquipmentID;
    public List<string> disableEquipmentID;
    public List<string> needFinishEpisodeID;
    public List<string> needItemID;
    public EpisodeType episodeType;
    public bool isNeedRecord;
    public BelongPhoneGroup belongGroup;
}

public enum BelongPhoneGroup
{
    None,
    MainRoleBoy,
    QingQian,
}

public class DialogConfig
{
    public string ID;
    public string content;
    public List<string> getItemID;
    public RoleType roleType;
    public List<string> choices;
    public bool isNeedRecord;
    public BelongPhoneGroup belongGroup;
}

public class ChoiceConfig
{
    public string ID;
    public string content;
    public string triggerEpisodeID;
}

public class EquipmentConfig
{
    public string ID;
    public string name;
    public string description;
    public List<string> getItemID;
    public string triggerEpisodeID;
    public string triggerPuzzleID;
    public List<string> mustDoneEpisodeID;
}

public class ItemConfig
{
    public string ID;
    public string name;
    public string description;
    public bool isSaveBag;
}

public class MergeClueConfig
{
    public string ID;
    public List<string> needFinishEpisodeID;
    public List<string> prepareClueList;
    public List<string> correctClueList;
    public string completeClue;
}


public class ClickPointConfig
{
    public string ID;
    public List<string> mustHaveItemID;
    public List<string> mustDoneEpisodeID;
    public string satisfyTriggerEpisodeID;
    public string unSatisfyTriggerEpisodeID;
    public string satisfyClickAgainTriggerEpisodeID;
    public bool isCorrect;
    public List<string> getItemID;
}

public enum RoleType
{
    None,
    MainRoleGirl,
    MainRoleBoy,
    QingQian,
}

public enum EpisodeType
{
    Normal,
    Phone
}

public enum PuzzleType
{
    JewelryPuzzleDone = 0,//������
    MimaPuzzleDone = 1//����
}

public class CharacterAutoMoveConfig
{
    public string ID;
    public float posX;
    public float posY;
    public float duration;
}
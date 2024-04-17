using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ConfigController : Singleton<ConfigController>
{
    private const string configPath = "Assets/Resources/Configs/";
    private const string fileTailPath = ".csv";
    private const string episodeConfigFilePath = configPath + "�����ĵ�_-_���" + fileTailPath;
    private const string dialogConfigFilePath = configPath + "�����ĵ�_-_�Ի�" + fileTailPath;
    private const string choiceConfigFilePath = configPath + "�����ĵ�_-_�Ի�ѡ��" + fileTailPath;
    private const string equipmentConfigFilePath = configPath + "�����ĵ�_-_��Ʒ" + fileTailPath;
    private const string itemConfigFilePath = configPath + "�����ĵ�_-_����" + fileTailPath;
    private const string clickPointConfigFilePath = configPath + "�����ĵ�_-_���������ť" + fileTailPath;
    private Dictionary<string, string> dataFilePathDic = new Dictionary<string, string>() {
        {"ChapterConfig", configPath + "�����ĵ�_-_�½�" + fileTailPath},
        //{"EpisodeConfig", ""},
    };

    public int normalTypingSpeed = 5;
    public int maxTypingSpeed = 10;
    private Dictionary<string, DataTable> datatableDic = new Dictionary<string, DataTable>();
    private Dictionary<string, ChapterConfig> chapterConfigList = new Dictionary<string, ChapterConfig>();  // �½�����
    private List<EpisodeConfig> episodeConfigList = new List<EpisodeConfig>();  // ��ͨ�Ի��������
    private List<PhoneEpisodeConfig> phoneEpisodeConfigList = new List<PhoneEpisodeConfig>();  // �ֻ��Ի��������
    private List<DialogConfig> dialogConfigList = new List<DialogConfig>();  // �Ի�����
    private List<ChoiceConfig> choiceConfigList = new List<ChoiceConfig>();  // ѡ������
    private List<EquipmentConfig> equipmentConfigList = new List<EquipmentConfig>();  // �����豸����Ʒ������
    private List<ItemConfig> itemConfigList = new List<ItemConfig>();  // ���ߣ�����������
    private List<ClickPointConfig> clickPointConfigs = new List<ClickPointConfig>(); // ���������ť

    public ConfigController()
    {
        GenerateConfig();
        //generateChapterConfig();
        //generateEpisodeConfig();
        //generateDialogConfig();
        //generateChoiceConfig();
        //generateEquipmentConfig();
        //generateItemConfig();
        //generateClickPointConfig();
    }

    public ChapterConfig GetChapterConfig(string chapterID)
    {
        if (!chapterConfigList.ContainsKey(chapterID))
        {
            ChapterConfig config = new ChapterConfig();
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

    public EpisodeConfig GetNormalEpisode(string episodeID)
    {
        foreach (var episode in episodeConfigList)
        {
            if (episode.ID == episodeID)
            {
                return episode;
            }
        }
        return null;
    }

    public DialogConfig GetDialog(string dialogID)
    {
        foreach (var dialog in dialogConfigList)
        {
            if (dialog.ID == dialogID)
            {
                return dialog;
            }
        }
        return null;
    }

    public ChoiceConfig GetChoice(string choiceID)
    {
        foreach (var choice in choiceConfigList)
        {
            if (choice.ID == choiceID)
            {
                return choice;
            }
        }
        return null;
    }

    public EquipmentConfig GetEquipment(string equipmentID)
    {
        foreach (var equipment in equipmentConfigList)
        {
            if (equipment.ID == equipmentID)
            {
                return equipment;
            }
        }
        return null;
    }

    public ItemConfig GetItem(string itemID)
    {
        foreach (var item in itemConfigList)
        {
            if (item.ID == itemID)
            {
                return item;
            }
        }
        return null;
    }

    private void GenerateConfig()
    {
        foreach (var item in dataFilePathDic)
        {
            string filePath = item.Value;
            DataTable dt = new DataTable();
            //�ļ�����ȡ
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs, Encoding.GetEncoding("gb2312"));
            string tempText = "";
            bool isFirst = true;
            while ((tempText = sr.ReadLine()) != null)
            {
                string[] arr = tempText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //һ���һ��Ϊ����
                if (isFirst)
                {
                    foreach (string str in arr)
                    {
                        dt.Columns.Add(str);
                    }
                    isFirst = false;
                }
                else
                {
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
    }
}

public class GameLineConfig
{
    public GameNodeType type;
    public string ID;
}

public enum GameNodeType
{
    GameStart,  // ��Ϸ��ʼ
    Transition,  // ת��
    NormalEpisode,  // ��ͨ�Ի�
    PhoneEpisode,  // �ֻ��Ի�
    Tutorial,  // �̳�
    FreeOperate,  // ���ɲ���
    Puzzle,  // ����
    GameEnd,  // ��Ϸ����
}

public class ChapterConfig
{
    public string ID;
    public string name;

    public ChapterConfig()
    {
        ID = "";
        name = "";
    }
}

public class EpisodeConfig
{
    public string ID;
    public List<string> dialogList;
    public List<string> enableEquipmentID;
    public List<string> disableEquipmentID;
}

public class PhoneEpisodeConfig : EpisodeConfig
{
    
}

public class DialogConfig
{
    public string ID;
    public string content;
    public string nextDialogID;
    public List<string> getItemID;
    public RoleType character;
    public bool isLeft;
    public List<string> choices;
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
    public string imageUrl;
    public bool isCollider;
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
    public string imageUrl;
}

public class PuzzleConfig
{
    public string ID;
    public PuzzleType type;
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

public enum Scene
{
    Scene1,
}

public enum RoleType
{
    MainRoleGirl,
    MainRoleBoy,
}

public enum TransitionType  // ת������
{
    GameStart,
    Blackout,  // ͣ��
    ChangeScene,
    ChangeCharacter
}

public enum PuzzleType
{

}
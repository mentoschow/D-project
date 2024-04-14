using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    private Dictionary<int, ChapterConfig> chapterConfigList = new Dictionary<int, ChapterConfig>();  // �½�����
    private List<EpisodeConfig> episodeConfigList = new List<EpisodeConfig>();  // �������
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

    public ChapterConfig GetChapterConfig (int chapterID)
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

    public EpisodeConfig GetEpisode(int episodeID)
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

    public DialogConfig GetDialog(int dialogID)
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

    public ChoiceConfig GetChoice(int choiceID)
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

    public EquipmentConfig GetEquipment(int equipmentID)
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

    public ItemConfig GetItem(int itemID)
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

    private void generateEpisodeConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(episodeConfigFilePath, System.IO.FileMode.Open);
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

                }
                isFirst = false;
            }
            else
            {
                EpisodeConfig config = new EpisodeConfig();
                config.ID = int.Parse(arr[0]);
                string[] list = arr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list.Length; i++)
                {
                    config.dialogList.Add(int.Parse(list[i]));
                }
                config.episodeType = (EpisodeType)int.Parse(arr[2]);
                string[] list2 = arr[3].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list2.Length; i++)
                {
                    config.enableEquipmentID.Add(int.Parse(list[i]));
                }
                string[] list3 = arr[4].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list3.Length; i++)
                {
                    config.disableEquipmentID.Add(int.Parse(list[i]));
                }

                episodeConfigList.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
    }

    private void generateDialogConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(dialogConfigFilePath, System.IO.FileMode.Open);
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

                }
                isFirst = false;
            }
            else
            {
                DialogConfig config = new DialogConfig();
                config.ID = int.Parse(arr[0]);
                config.content = arr[1];
                config.nextDialogID = int.Parse(arr[2]);
                string[] list = arr[3].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list.Length; i++)
                {
                    config.getItemID.Add(int.Parse(list[i]));
                }
                config.character = (Character)int.Parse(arr[4]);
                config.isLeft = bool.Parse(arr[5]);
                string[] list2 = arr[6].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list2.Length; i++)
                {
                    config.choices.Add(int.Parse(list[i]));
                }

                dialogConfigList.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
    }

    private void generateChoiceConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(choiceConfigFilePath, System.IO.FileMode.Open);
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

                }
                isFirst = false;
            }
            else
            {
                ChoiceConfig config = new ChoiceConfig();
                config.ID = int.Parse(arr[0]);
                config.content = arr[1];
                config.triggerEpisodeID = int.Parse(arr[2]);

                choiceConfigList.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
    }

    private void generateEquipmentConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(equipmentConfigFilePath, System.IO.FileMode.Open);
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

                }
                isFirst = false;
            }
            else
            {
                EquipmentConfig config = new EquipmentConfig();
                config.ID = int.Parse(arr[0]);
                config.name = arr[1];
                config.description = arr[2];
                config.imageUrl = arr[3];
                config.isCollider = bool.Parse(arr[4]);
                string[] list = arr[5].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list.Length; i++)
                {
                    config.getItemID.Add(int.Parse(list[i]));
                }
                config.triggerEpisodeID = int.Parse(arr[6]);
                config.triggerPuzzleID = int.Parse(arr[7]);
                string[] list2 = arr[8].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list2.Length; i++)
                {
                    config.mustDoneEpisodeID.Add(int.Parse(list[i]));
                }

                equipmentConfigList.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
    }

    private void generateItemConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(itemConfigFilePath, System.IO.FileMode.Open);
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

                }
                isFirst = false;
            }
            else
            {
                ItemConfig config = new ItemConfig();
                config.ID = int.Parse(arr[0]);
                config.name = arr[1];
                config.description = arr[2];
                config.imageUrl = arr[3];

                itemConfigList.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
    }

    private void generateClickPointConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(clickPointConfigFilePath, System.IO.FileMode.Open);
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

                }
                isFirst = false;
            }
            else
            {
                ClickPointConfig config = new ClickPointConfig();
                config.ID = int.Parse(arr[0]);
                string[] list = arr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list.Length; i++)
                {
                    config.mustHaveItemID.Add(int.Parse(list[i]));
                }
                string[] list2 = arr[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list2.Length; i++)
                {
                    config.mustDoneEpisodeID.Add(int.Parse(list[i]));
                }
                config.satisfyTriggerEpisodeID = int.Parse(arr[3]);
                config.unSatisfyTriggerEpisodeID = int.Parse(arr[4]);
                config.satisfyClickAgainTriggerEpisodeID = int.Parse(arr[5]);
                config.isCorrect = bool.Parse(arr[6]);
                string[] list3 = arr[7].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list3.Length; i++)
                {
                    config.getItemID.Add(int.Parse(list[i]));
                }

                clickPointConfigs.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
    }
}

public class ChapterConfig
{
    public int ID;
    public string name;
}

public class EpisodeConfig
{
    public int ID;
    public List<int> dialogList;
    public EpisodeType episodeType;
    public List<int> enableEquipmentID;
    public List<int> disableEquipmentID;
}

public class DialogConfig
{
    public int ID;
    public string content;
    public int nextDialogID;
    public List<int> getItemID;
    public Character character;
    public bool isLeft;
    public List<int> choices;
    public EpisodeType episodeType;
}

public class ChoiceConfig
{
    public int ID;
    public string content;
    public int triggerEpisodeID;
    public EpisodeType episodeType;
}

public class EquipmentConfig
{
    public int ID;
    public string name;
    public string description;
    public string imageUrl;
    public bool isCollider;
    public List<int> getItemID;
    public int triggerEpisodeID;
    public int triggerPuzzleID;
    public List<int> mustDoneEpisodeID;
}

public class ItemConfig
{
    public int ID;
    public string name;
    public string description;
    public string imageUrl;
}

public class ClickPointConfig
{
    public int ID;
    public List<int> mustHaveItemID;
    public List<int> mustDoneEpisodeID;
    public int satisfyTriggerEpisodeID;
    public int unSatisfyTriggerEpisodeID;
    public int satisfyClickAgainTriggerEpisodeID;
    public bool isCorrect;
    public List<int> getItemID;
}

public enum Scene
{
    None,
    Scene1,
}

public enum Character
{
    MainRole,
}

public enum EpisodeType
{
    Normal,
    Phone
}
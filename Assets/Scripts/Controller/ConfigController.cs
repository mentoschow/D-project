using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class ConfigController
{
    private const string configPath = "Assets/Resources/Configs/";
    private const string fileTailPath = ".csv";
    private const string chapterConfigFilePath = configPath + "�����ĵ�_-_�½�" + fileTailPath;
    private const string episodeConfigFilePath = configPath + "�����ĵ�_-_���" + fileTailPath;
    private const string dialogConfigFilePath = configPath + "�����ĵ�_-_�Ի�" + fileTailPath;
    private const string choiceConfigFilePath = configPath + "�����ĵ�_-_�Ի�ѡ��" + fileTailPath;
    private const string equipmentConfigFilePath = configPath + "�����ĵ�_-_��Ʒ" + fileTailPath;
    private const string itemConfigFilePath = configPath + "�����ĵ�_-_����" + fileTailPath;

    public List<Scene> scenes = new List<Scene>();
    public List<GameObject> sceneGameObjs = new List<GameObject>();
    public List<Chapter> chapterLine;  // ��Ϸ�½�����
    public int normalTypingSpeed = 5;
    public int maxTypingSpeed = 10;
    private List<ChapterConfig> chapterConfigList = new List<ChapterConfig>();  // �½�����
    private List<EpisodeConfig> episodeConfigList = new List<EpisodeConfig>();  // �������
    private List<DialogConfig> dialogConfigList = new List<DialogConfig>();  // �Ի�����
    private List<ChoiceConfig> choiceConfigList = new List<ChoiceConfig>();  // ѡ������
    private List<EquipmentConfig> equipmentConfigList = new List<EquipmentConfig>();  // �����豸����Ʒ������
    private List<ItemConfig> itemConfigList = new List<ItemConfig>();  // ���ߣ�����������

    public void InitConfig()
    {
        generateChapterConfig();
        generateEpisodeConfig();
        generateDialogConfig();
        generateChoiceConfig();
        generateEquipmentConfig();
        generateItemConfig();
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

    private void generateChapterConfig()
    {
        //�ļ�����ȡ
        System.IO.FileStream fs = new System.IO.FileStream(chapterConfigFilePath, System.IO.FileMode.Open);
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
                ChapterConfig config = new ChapterConfig();
                config.ID = (Chapter)int.Parse(arr[0]);
                config.name = arr[1];
                chapterConfigList.Add(config);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();
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
}

public class ChapterConfig
{
    public Chapter ID;
    public string name;
}

public class EpisodeConfig
{
    public int ID;
    public List<int> dialogList;
    public EpisodeType episodeType;
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
}

public class ItemConfig
{
    public int ID;
    public string name;
    public string description;
    public string imageUrl;
}

public enum Chapter
{
    Chapter1,
    Chapter2,
    Chapter3
}

public enum Scene
{
    Scene1
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
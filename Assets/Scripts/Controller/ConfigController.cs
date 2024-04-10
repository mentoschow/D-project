using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class ConfigController : MonoBehaviour
{
    private const string configPath = "Assets/Resources/Configs/";
    private const string fileTailPath = ".csv";
    private const string chapterConfigFilePath = configPath + "�����ĵ�_-_�½�" + fileTailPath;

    public GameConfig gameConfig = new GameConfig();  // ��Ϸ�½�����
    public List<ChapterConfig> chapterConfigList = new List<ChapterConfig>();  // �½�����
    public List<EpisodeConfig> episodeConfigList = new List<EpisodeConfig>();  // �������
    public List<DialogConfig> dialogConfigList = new List<DialogConfig>();  // �Ի�����
    public List<ChoiceConfig> choiceConfigList = new List<ChoiceConfig>();  // ѡ������
    public List<EquipmentConfig> equipmentConfigList = new List<EquipmentConfig>();  // �����豸����Ʒ������
    public List<ItemConfig> itemConfigList = new List<ItemConfig>();  // ���ߣ�����������

    private void Start()
    {
        generateChapterConfig();
        //foreach(var chapterConfig in chapterConfigList)
        //{
        //    Debug.Log(chapterConfig.name);
        //    Debug.Log(chapterConfig.triggerEpisodeID);
        //}
    }

    private void generateChapterConfig()
    {
        DataTable dt = new DataTable();
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
                    dt.Columns.Add(str);
                }
                isFirst = false;
            }
            else
            {
                //�ӵڶ��п�ʼ��ȡ
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dr[i] = i < arr.Length ? arr[i] : "";
                }
                dt.Rows.Add(dr);
            }
        }
        //�ر���
        sr.Close();
        fs.Close();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            string str = dt.Columns[i].ToString();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                var a = dt.Rows[i];
                var b = dt.Rows[i][str];
                string strName = dt.Rows[i][str].ToString();
                Debug.Log(strName);
            }
        }
    }
}

public class GameConfig
{
    public List<Chapter> chapterConfigList;
}

public class ChapterConfig
{
    public Chapter ID;
    public string name;
    public int triggerEpisodeID;
}

public class EpisodeConfig
{
    public List<int> dialogList;
    public EpisodeType episodeType;
}

public class DialogConfig
{
    public int ID;
    public string content;
    public int nextDialogID;
    public int getItemID;
    public Character character;
    public bool isLeft;
    public List<ChoiceConfig> choices;
}

public class ChoiceConfig
{
    public int ID;
    public string content;
    public int triggerEpisodeID;
}

public class EquipmentConfig
{
    public int ID;
    public string name;
    public string description;
    public string imageUrl;
    public bool isCollider;
    public int getItemID;
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
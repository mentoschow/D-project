using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRes", menuName = "ScriptableObjects/GameRes", order = 1)]
public class GameRes : ScriptableObject
{
    [Header("��������ͼƬ��Դ")]
    public List<ImageRes> clueItemImage = new List<ImageRes>();
    [Header("�Ի��е�ͼƬ��Դ")]
    public List<ImageRes> dialogImage = new List<ImageRes>();
}

[System.Serializable]
public class ImageRes
{
    public string ID;
    public Sprite sprite;
}

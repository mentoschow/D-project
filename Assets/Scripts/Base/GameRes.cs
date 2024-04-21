using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRes", menuName = "ScriptableObjects/GameRes", order = 1)]
public class GameRes : ScriptableObject
{
    public List<ImageRes> imageRes = new List<ImageRes>();
}

[System.Serializable]
public class ImageRes
{
    public string name;
    public Sprite sprite;
}

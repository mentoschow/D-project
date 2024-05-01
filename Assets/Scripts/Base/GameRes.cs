using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRes", menuName = "ScriptableObjects/GameRes", order = 1)]
public class GameRes : ScriptableObject
{
    [Header("场景资源")]
    public List<SceneRes> scenePrefabs = new List<SceneRes>();
    [Header("线索道具图片资源")]
    public List<ImageRes> clueItemImage = new List<ImageRes>();
    [Header("对话中的图片资源")]
    public List<ImageRes> dialogImage = new List<ImageRes>();
    [Header("角色图片资源")]
    public List<RoleRes> roleRes = new List<RoleRes>();
    [Header("聊聊分组资源")]
    public List<WechatGroupRes> wechatGroupRes = new List<WechatGroupRes>();
}

[System.Serializable]
public class ImageRes
{
    public string ID;
    public Sprite sprite;
}
[System.Serializable]
public class RoleRes
{
    public RoleType type;
    public string name;
    public Sprite icon;
    public Sprite fullBody;
}

[System.Serializable]
public class WechatGroupRes
{
    public BelongPhoneGroup group;
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class SceneRes
{
    public StageType type;
    public GameObject prefab;
    public List<float> bornPosX;
}

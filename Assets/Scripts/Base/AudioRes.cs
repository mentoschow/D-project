using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRes", menuName = "ScriptableObjects/AudioRes", order = 2)]
public class AudioRes : ScriptableObject
{
    [Header("��Ϸ��Ч")]
    public List<AudioClipRes> effectRes;
    [Header("��������")]
    public List<AudioClipRes> bgmRes;
}
[System.Serializable]
public class AudioClipRes
{
    public AudioType type;
    public AudioClip clip;
    public float volumn;
}

public enum AudioType
{
    // effect
    GameStartButton,
    NormalButton,
    NormalDialogPlayButton,
    TypeEffect,
    PhoneDialogPlayButton,
    PhoneButton,
    DragStart,
    Adsorbed,  // ����
    PuzzleWrong,
    PuzzleCorrect,
    StartPhoneEpisode,
    Clock,
    BlackOut,
    ClosetMove,

    //bgm
    NormalBgm,
    BlackOutBgm,
    PassBgm,
    NowBgm
}
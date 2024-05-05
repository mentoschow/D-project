using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRes", menuName = "ScriptableObjects/AudioRes", order = 2)]
public class AudioRes : ScriptableObject
{
    [Header("”Œœ∑“Ù–ß")]
    public List<AudioEffectRes> effectRes;
}
[System.Serializable]
public class AudioEffectRes
{
    public AudioEffectType type;
    public AudioClip clip;
    public float volumn;
}

public enum AudioEffectType
{
    GameStartButton,
    NormalButton,
    NormalDialogPlayButton,
    TypeEffect,
    PhoneDialogPlayButton,
    PhoneButton,
    MergeClueButton,
    Adsorbed,  // Œ¸∏Ω
    PuzzleWrong,
    PuzzleCorrect,
}
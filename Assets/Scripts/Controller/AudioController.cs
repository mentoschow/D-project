using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoSingleton<AudioController>
{
    [SerializeField]
    private AudioRes audioRes;
    [SerializeField]
    private AudioSource effectPlayer;
    [SerializeField]
    private AudioSource bgmPlayer;

    private Dictionary<AudioType, AudioClipRes> effectRes = new Dictionary<AudioType, AudioClipRes>();
    private Dictionary<AudioType, AudioClipRes> bgmRes = new Dictionary<AudioType, AudioClipRes>();

    void Start()
    {
        foreach (var res in audioRes.effectRes)
        {
            effectRes[res.type] = res;
        }
        foreach (var res in audioRes.bgmRes)
        {
            bgmRes[res.type] = res;
        }
    }

    public void PlayAudioEffect(AudioType type, bool loop = false)
    {
        if (effectRes.ContainsKey(type))
        {
            effectPlayer.loop = loop;
            effectPlayer.PlayOneShot(effectRes[type].clip, effectRes[type].volumn);
        }
    }

    public void PlayBgm(AudioType type)
    {
        if (bgmRes.ContainsKey(type))
        {
            bgmPlayer.loop = true;
            bgmPlayer.clip = bgmRes[type].clip;
            bgmPlayer.volume = bgmRes[type].volumn;
            bgmPlayer.Play();
        }
    }

    public void Stop()
    {
        effectPlayer.Stop();
    }
}

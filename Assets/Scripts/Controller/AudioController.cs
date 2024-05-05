using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoSingleton<AudioController>
{
    [SerializeField]
    private AudioRes audioRes;
    [SerializeField]
    private AudioSource audioSource;

    private Dictionary<AudioEffectType, AudioEffectRes> effectRes = new Dictionary<AudioEffectType, AudioEffectRes>();

    void Start()
    {
        foreach (var res in audioRes.effectRes)
        {
            effectRes[res.type] = res;
        }
    }

    public void PlayAudioEffect(AudioEffectType type, bool loop = false)
    {
        if (effectRes.ContainsKey(type))
        {
            audioSource.loop = loop;
            audioSource.PlayOneShot(effectRes[type].clip, effectRes[type].volumn);
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}

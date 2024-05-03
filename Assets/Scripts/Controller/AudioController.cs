using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoSingleton<AudioController>
{
    [SerializeField]
    private AudioRes audioRes;
    [SerializeField]
    private AudioSource audioSource;

    private Dictionary<AudioEffectType, AudioClip> effectRes = new Dictionary<AudioEffectType, AudioClip>();

    void Start()
    {
        foreach (var res in audioRes.effectRes)
        {
            effectRes[res.type] = res.clip;
        }
    }

    public void PlayAudioEffect(AudioEffectType type, bool loop = false)
    {
        if (effectRes.ContainsKey(type))
        {
            audioSource.loop = loop;
            audioSource.PlayOneShot(effectRes[type]);
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}

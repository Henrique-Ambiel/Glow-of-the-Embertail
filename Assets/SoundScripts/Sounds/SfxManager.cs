using UnityEngine;
using System;
using System.Collections.Generic;

public enum SoundType
{
    Click,
    Waterdrip,
    Candle,
    Score,
    NextLevel,
    Death,
    Hover
}

[Serializable]
public class SoundSFX
{
    public SoundType type;
    public AudioClip clip;
}

public class SfxManager : MonoBehaviour
{
    [SerializeField] private SoundEventsData soundEvents;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private  List<SoundSFX> soundSFX = new();
    
    private Dictionary<SoundType, AudioClip> soundDataBase = new();

    private void Awake()
    {
        AddDictionarySound(soundSFX);
    }

    private void OnEnable()
    {
        soundEvents.OnPlaySfx += PlaySound;
    }

    private void OnDisable()
    {
        soundEvents.OnPlaySfx -= PlaySound;
    }

    private void AddDictionarySound(List<SoundSFX> soundList)
    {
        foreach (SoundSFX sound in soundList)
        {
            soundDataBase.Add(sound.type, sound.clip);
        }
    }

    private void PlaySound(SoundType type)
    {
        audioSource.PlayOneShot(soundDataBase[type]);
    }
}

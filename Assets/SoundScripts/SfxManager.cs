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
    Death
}

[Serializable]
public class SoundSFX
{
    public SoundType type;
    public AudioClip clip;
}

public class SfxManager : MonoBehaviour
{
    public SoundType soundTypeTest;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private  List<SoundSFX> soundSFX = new();
    
    private Dictionary<SoundType, AudioClip> soundDataBase = new();

    private void Awake()
    {
        AddDictionarySound(soundSFX);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlaySound(soundTypeTest);
        }
    }

    private void OnEnable()
    {
        SoundEvents.Instance.OnPlaySfx += PlaySound;
    }

    private void OnDisable()
    {
        SoundEvents.Instance.OnPlaySfx -= PlaySound;
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

using UnityEngine;
using System;

public class SoundEvents : MonoBehaviour
{
    public static SoundEvents Instance;

    private void Awake()
    {
        Instance = this;
    }

    public event Action<SoundType> OnPlaySfx; 
    public event Action<int> OnPlayMusic;
    
    public void PlaySFX(SoundType soundType) => OnPlaySfx?.Invoke(soundType);
    
    public void PlayMusic(int id) => OnPlayMusic?.Invoke(id);
}

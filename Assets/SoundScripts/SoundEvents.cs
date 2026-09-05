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
    
    public void PlaySFX(SoundType soundType) => OnPlaySfx?.Invoke(soundType);
}

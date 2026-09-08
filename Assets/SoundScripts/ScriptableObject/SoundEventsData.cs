using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Sound Events Data", menuName = "Sound Data/New Sound Data")]
public class SoundEventsData : ScriptableObject
{
    public event Action<SoundType> OnPlaySfx; 
    public event Action<int> OnPlayMusic;
    
    public void PlaySFX(SoundType soundType) => OnPlaySfx?.Invoke(soundType);
    
    public void PlayMusic(int id) => OnPlayMusic?.Invoke(id);
}

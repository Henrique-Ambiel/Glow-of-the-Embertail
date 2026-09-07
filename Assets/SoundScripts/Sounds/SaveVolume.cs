using UnityEngine;
using System.Collections.Generic;

public class SaveVolume : MonoBehaviour
{
    
    private VolumeSettingsSave saveSettings;
    
    public void SaveVolumeData(VolumeType soundType,float volume)
    {
        VolumeSettings v = new VolumeSettings
        {
            soundType = soundType,
            volume = volume
        };
    }
}

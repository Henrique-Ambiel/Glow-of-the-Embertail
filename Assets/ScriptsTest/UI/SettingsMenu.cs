using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Value Text")]
    [SerializeField] private TMP_Text masterValueText;
    [SerializeField] private TMP_Text musicValueText;
    [SerializeField] private TMP_Text sfxValueText;

    [Header("Video")]
    [SerializeField] private Toggle fullscreenToggle;
    
    
    [Header("Save Reference")]
    [SerializeField] private SaveVolume saveVolume;
    
    private void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        
        StartVolumeSlider();
    }

    public void UpdateMasterVolume(float value)
    {
        masterSlider.value = value;
        SetMixerVolume("Master", value);
        masterValueText.text =
            Mathf.RoundToInt(value * 100) + "%";
        saveVolume.SaveVolumeData(VolumeType.Master, value);
    }

    public void UpdateMusicVolume(float value)
    {
        musicSlider.value = value;
        SetMixerVolume("MusicVolume", value);

        musicValueText.text =
            Mathf.RoundToInt(value * 100) + "%";
        saveVolume.SaveVolumeData(VolumeType.Music, value);
    }

    public void UpdateSFXVolume(float value)
    {
        sfxSlider.value = value;
        SetMixerVolume("SFXVolume", value);

        sfxValueText.text =
            Mathf.RoundToInt(value * 100) + "%";
        saveVolume.SaveVolumeData(VolumeType.Sfx, value);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    private void SetMixerVolume(string parameter, float value)
    {
        float volume = Mathf.Log10(
            Mathf.Max(value, 0.0001f)
        ) * 20f;

        audioMixer.SetFloat(parameter, volume);
    }

    public void StartVolumeSlider()
    {
        VolumeSettingsSave volumeData = saveVolume.GetVolumeData();

        foreach (var v in volumeData.volumeList)
        {
            Debug.Log(v.volume);
            Debug.Log(v.soundType);
        }

        if (volumeData.volumeList.Count == 0)
        {
                UpdateMasterVolume(1f);
                UpdateMusicVolume(1f);
                UpdateSFXVolume(1f);
        }
            
        foreach (var v in volumeData.volumeList)
        {
                
            switch (v.soundType)
            {
                case VolumeType.Master:
                    masterSlider.value = v.volume;
                    UpdateMasterVolume(v.volume);
                    Debug.Log(v.volume);
                    break;

                case VolumeType.Music:
                    musicSlider.value = v.volume;
                    UpdateMusicVolume(v.volume);
                    Debug.Log(v.volume);
                    break;

                case VolumeType.Sfx:
                    sfxSlider.value = v.volume;
                    UpdateSFXVolume(v.volume);
                    Debug.Log(v.volume);
                    break;
            }
        }
    }
}
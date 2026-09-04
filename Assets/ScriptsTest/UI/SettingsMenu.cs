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

    private void Start()
    {
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;

        fullscreenToggle.isOn = Screen.fullScreen;

        UpdateMasterVolume(1f);
        UpdateMusicVolume(1f);
        UpdateSFXVolume(1f);
    }

    public void UpdateMasterVolume(float value)
    {
        SetMixerVolume("Master", value);

        masterValueText.text =
            Mathf.RoundToInt(value * 100) + "%";
    }

    public void UpdateMusicVolume(float value)
    {
        SetMixerVolume("MusicVolume", value);

        musicValueText.text =
            Mathf.RoundToInt(value * 100) + "%";
    }

    public void UpdateSFXVolume(float value)
    {
        SetMixerVolume("SFXVolume", value);

        sfxValueText.text =
            Mathf.RoundToInt(value * 100) + "%";
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
}
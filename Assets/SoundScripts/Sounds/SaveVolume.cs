using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Debug = UnityEngine.Debug;

public class SaveVolume : MonoBehaviour
{
    [SerializeField] private SettingsMenu settingsMenu;
    
    private Dictionary<VolumeType, float> volumeSettingsData = new Dictionary<VolumeType, float>();
    private VolumeSettingsSave saveSettings = new VolumeSettingsSave();

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "volume.json");


    private void Awake()
    {
        Load();
        settingsMenu.StartVolumeSlider();
    }


    public void SaveVolumeData(VolumeType soundType, float volume)
    {
        volumeSettingsData[soundType] = volume;
    }

    private void UpdateVolumeList()
    {
        saveSettings.volumeList.Clear();

        foreach (var v in volumeSettingsData)
        {
            VolumeSettings s = new VolumeSettings
            {
                soundType = v.Key,
                volume = v.Value
            };

            saveSettings.volumeList.Add(s);
        }
    }

    public void Save()
    {
        UpdateVolumeList();

        try
        {
            string json = JsonUtility.ToJson(saveSettings, true);
            File.WriteAllText(SavePath, json);
#if UNITY_EDITOR
            Debug.Log("Volume salvo com sucesso.");
#endif
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Falha ao salvar volume: {e}");
#endif
        }
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
            return;

        try
        {
            string json = File.ReadAllText(SavePath);
            var loaded = JsonUtility.FromJson<VolumeSettingsSave>(json);

            saveSettings = loaded ?? new VolumeSettingsSave();

            volumeSettingsData.Clear();

            foreach (var v in saveSettings.volumeList)
            {
                volumeSettingsData[v.soundType] = v.volume;
            }

#if UNITY_EDITOR
            Debug.Log("Volume carregado com sucesso.");
#endif
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Falha ao carregar volume: {e}");
#endif
            saveSettings = new VolumeSettingsSave();
        }
    }

    public void OpenSaveFolder()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    public VolumeSettingsSave GetVolumeData()
    {
        UpdateVolumeList();
        return saveSettings;
    }
}

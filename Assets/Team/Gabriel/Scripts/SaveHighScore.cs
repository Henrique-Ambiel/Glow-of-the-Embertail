using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class HighScoreList
{
    public List<HighScore> highScores;
}

[Serializable]
public class HighScore
{
    public string note;
    public float time;
    public string data;
}

public class SaveHighScore : MonoBehaviour
{
    private HighScoreList highScores = new HighScoreList();
    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "HighScore.json");

    private void Awake()
    {
        Load();
    }

    public void UpdateScore(string note, float time, string data)
    {
        HighScore score = new HighScore
        {
            note = note,
            time = time,
            data = data
        };
        
        highScores.highScores.Add(score);
        
        SortHighScores();
    }

    private void SortHighScores()
    {
        highScores.highScores.Sort((b,a) => a.time.CompareTo(b.time));

        if (highScores.highScores.Count > 4)
            highScores.highScores.RemoveAt(highScores.highScores.Count - 1);
        
        Save();
    }

    private void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(highScores, true);
            File.WriteAllText(SavePath, json);
#if UNITY_EDITOR
            Debug.Log("High Score salvo com sucesso.");
#endif
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Falha ao salvar HighScore: {e}");
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
            var loaded = JsonUtility.FromJson<HighScoreList>(json);

            highScores = loaded ?? new HighScoreList();

            highScores = loaded;

#if UNITY_EDITOR
            Debug.Log("Volume carregado com sucesso.");
#endif
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Falha ao carregar volume: {e}");
#endif
            highScores = new HighScoreList();
        }
    }

    public IReadOnlyList<HighScore> GetHighScores()
    {
        return highScores.highScores;
    }
}

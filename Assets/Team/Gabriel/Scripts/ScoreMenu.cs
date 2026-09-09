using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreMenu : MonoBehaviour
{
    [SerializeField] private SaveHighScore scoreSave;
    
    [SerializeField] private TMP_Text score1;
    [SerializeField] private TMP_Text score2;
    [SerializeField] private TMP_Text score3;
    [SerializeField] private TMP_Text score4;
    
    private List<HighScore> highScores;

    private void UpdateUI()
    {
        IReadOnlyList<HighScore> s = scoreSave.GetHighScores();

        if (s.Count == 1)
        {
            
        }
    }
    
    private string FormatTime(string note, float time, string date)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int centiseconds =
            Mathf.FloorToInt((time % 1f) * 100f);

        return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
    }
}

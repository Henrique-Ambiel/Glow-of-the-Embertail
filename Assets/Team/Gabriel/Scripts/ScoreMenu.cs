using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreMenu : MonoBehaviour
{
    [SerializeField] private SaveHighScore scoreSave;
    
    //[SerializeField] private TMP_Text score1;
    //[SerializeField] private TMP_Text score2;
    //[SerializeField] private TMP_Text score3;
    //[SerializeField] private TMP_Text score4;
    
    [SerializeField] List<TMP_Text> highScores = new List<TMP_Text>();

    private void Start()
    {
        UpdateUI();
    }


    private void UpdateUI()
    {
        IReadOnlyList<HighScore> s = scoreSave.GetHighScores();

        for (int i = 0; i < highScores.Count; i++)
        {
            highScores[i].text = FormatText(s[i].note, s[i].time, s[i].date);
        }
        
    }
    
    private string FormatText(string note, float time, string date)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int centiseconds =
            Mathf.FloorToInt((time % 1f) * 100f);

        return $"{note} / {minutes:00}:{seconds:00}.{centiseconds:00} / {date}";
    }
}

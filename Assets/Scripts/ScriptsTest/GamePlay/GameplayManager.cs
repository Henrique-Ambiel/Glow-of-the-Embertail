using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public enum GameplayState
{
    Playing,
    Paused,
    Dead,
    Finished
}

public sealed class GameplayManager : MonoBehaviour
{
    [Header("Jogador")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private FlameManagerAdapter flameEnergy;

    [Header("Painéis")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("HUD")]
    [SerializeField] private TMP_Text timerText;
    //[SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text deathsText;
    [SerializeField] private TMP_Text candlesText;

    [Header("Resultado")]
    [SerializeField] private TMP_Text finalTimeText;
    //[SerializeField] private TMP_Text finalPointsText;
    [SerializeField] private TMP_Text finalDeathsText;
    [SerializeField] private TMP_Text finalCandlesText;
    [SerializeField] private TMP_Text rankText;

    [Header("Ranking")]
    [SerializeField, Min(1f)]
    private float targetTimeSeconds = 120f;

    [SerializeField, Min(1f)]
    private float maximumTimeSeconds = 300f;

    [SerializeField, Min(1)]
    private int deathsForZeroScore = 5;

    [Header("Cenas")]
    [SerializeField] private string menuSceneName = "Menu";

    private GameplayState currentState;
    private float elapsedTime;
    //private int points;
    private int deaths;
    private int totalCandles;
    private Transform activeRespawnPoint;
    private Coroutine respawnRoutine;

    private readonly HashSet<Candle> collectedCandles =
        new HashSet<Candle>();

    public int TotalCandles => totalCandles;
    public int CollectedCandles => collectedCandles.Count;

    public GameplayState CurrentState => currentState;
    public float ElapsedTime => elapsedTime;
    //public int Points => points;
    public int Deaths => deaths;

    private void Awake()
    {
        Time.timeScale = 1f;
        currentState = GameplayState.Playing;
        activeRespawnPoint = respawnPoint;

        SetPanel(pausePanel, false);
        SetPanel(deathPanel, false);
        SetPanel(resultPanel, false);

        RefreshTimer();
        RefreshCounters();
    }

    private void Start()
    {
        Candle[] sceneCandles =
            FindObjectsByType<Candle>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        totalCandles = sceneCandles.Length;
        RefreshCounters();
    }

    private void OnEnable()
    {
        if (flameEnergy != null)
            flameEnergy.Extinguished += HandleFlameExtinguished;
    }

    private void OnDisable()
    {
        if (flameEnergy != null)
            flameEnergy.Extinguished -= HandleFlameExtinguished;
    }

    private void Update()
    {
        HandlePauseInput();

        if (currentState != GameplayState.Playing)
            return;

        elapsedTime += Time.unscaledDeltaTime;
        RefreshTimer();
    }

    private void HandlePauseInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (currentState == GameplayState.Playing)
            PauseGame();
        else if (currentState == GameplayState.Paused)
            ResumeGame();
    }

    public void PauseGame()
    {
        if (currentState != GameplayState.Playing)
            return;

        currentState = GameplayState.Paused;
        SetPanel(pausePanel, true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (currentState != GameplayState.Paused)
            return;

        SetPanel(pausePanel, false);
        currentState = GameplayState.Playing;
        Time.timeScale = 1f;
    }

    private void HandleFlameExtinguished()
    {
        if (
            currentState != GameplayState.Playing ||
            respawnRoutine != null
        )
        {
            return;
        }

        deaths++;
        RefreshCounters();

        respawnRoutine =
            StartCoroutine(RespawnAtLastCheckpoint());
    }

    private IEnumerator RespawnAtLastCheckpoint()
    {
        currentState = GameplayState.Dead;

        SetPanel(pausePanel, false);
        SetPanel(deathPanel, false);

        Time.timeScale = 1f;

        // Aguarda o dash e o frame da morte terminarem.
        yield return null;

        MovePlayerToRespawn();

        if (flameEnergy != null)
        {
            flameEnergy.Restore(
                flameEnergy.Maximum
            );
        }

        currentState = GameplayState.Playing;
        respawnRoutine = null;
    }

    public void Respawn()
    {
        if (currentState != GameplayState.Dead)
            return;

        MovePlayerToRespawn();

        if (flameEnergy != null)
            flameEnergy.Restore(flameEnergy.Maximum);

        SetPanel(deathPanel, false);

        currentState = GameplayState.Playing;
        Time.timeScale = 1f;
    }

    private void MovePlayerToRespawn()
    {
        Transform targetPoint =
            activeRespawnPoint != null
                ? activeRespawnPoint
                : respawnPoint;

        if (player == null || targetPoint == null)
            return;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        bool controllerWasEnabled =
            controller != null && controller.enabled;

        if (controllerWasEnabled)
            controller.enabled = false;

        player.SetPositionAndRotation(
            targetPoint.position,
            targetPoint.rotation
        );

        if (controllerWasEnabled)
            controller.enabled = true;
    }

    /*public void AddPoints(int amount)
    {
        if (amount <= 0 || currentState == GameplayState.Finished)
            return;

        points += amount;
        RefreshCounters();
    }*/

    public void RegisterCandleCollected(Candle candle)
    {
        if (
            candle == null ||
            currentState != GameplayState.Playing
        )
        {
            return;
        }

        if (!collectedCandles.Add(candle))
            return;

        RefreshCounters();
    }

    public void FinishLevel()
    {
        if (currentState != GameplayState.Playing)
            return;

        currentState = GameplayState.Finished;
        Time.timeScale = 0f;

        SetPanel(pausePanel, false);
        SetPanel(deathPanel, false);
        SetPanel(resultPanel, true);

        RefreshResult();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private void RefreshTimer()
    {
        if (timerText != null)
            timerText.text = FormatTime(elapsedTime);
    }

    private void RefreshCounters()
    {
       /* if (pointsText != null)
            pointsText.text = $"Pontos: {points}";*/

        if (deathsText != null)
            deathsText.text = $"Mortes: {deaths}";

        if (candlesText != null)
        {
            candlesText.text =
                $"{collectedCandles.Count} / {totalCandles}";
        }
    }

    private void RefreshResult()
    {
        if (finalTimeText != null)
            finalTimeText.text =
                $"Tempo: {FormatTime(elapsedTime)}";

        /*if (finalPointsText != null)
            finalPointsText.text =
                $"Pontos: {points}";*/

        if (finalDeathsText != null)
            finalDeathsText.text =
                $"Mortes: {deaths}";

        if (finalCandlesText != null)
        {
            finalCandlesText.text =
                $"VELAS: {collectedCandles.Count} / {totalCandles}";
        }

        if (rankText != null)
            rankText.text = CalculateRank();
    }

    private string CalculateRank()
    {
        float timeProgress = Mathf.InverseLerp(
            targetTimeSeconds,
            maximumTimeSeconds,
            elapsedTime
        );

        float timeScore =
            (1f - timeProgress) * 100f;

        float deathScore =
            Mathf.Clamp01(
                1f - deaths / (float)deathsForZeroScore
            ) * 100f;

        float candleScore =
      totalCandles > 0
          ? collectedCandles.Count / (float)totalCandles * 100f
          : 100f;

        float finalScore =
            (timeScore + deathScore + candleScore) / 3f;

        if (finalScore >= 95f) return "A+";
        if (finalScore >= 90f) return "A";
        if (finalScore >= 85f) return "B+";
        if (finalScore >= 80f) return "B";
        if (finalScore >= 70f) return "C+";
        if (finalScore >= 60f) return "C";
        if (finalScore >= 50f) return "D+";
        if (finalScore >= 40f) return "D";

        return "E";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int centiseconds =
            Mathf.FloorToInt((time % 1f) * 100f);

        return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    private void OnValidate()
    {
        targetTimeSeconds =
            Mathf.Max(1f, targetTimeSeconds);

        maximumTimeSeconds =
            Mathf.Max(
                targetTimeSeconds + 1f,
                maximumTimeSeconds
            );

        deathsForZeroScore =
            Mathf.Max(1, deathsForZeroScore);
    }

    public void ActivateCheckpoint(Transform checkpointPoint)
    {
        if (
            currentState != GameplayState.Playing ||
            checkpointPoint == null
        )
        {
            return;
        }

        activeRespawnPoint = checkpointPoint;
    }

    public void ActivateCheckpoint(
        Candle candle,
        Transform checkpointPoint
    )
    {
        if (candle == null)
            return;

        RegisterCandleCollected(candle);
        ActivateCheckpoint(checkpointPoint);
    }
}
using System;
using UnityEngine;

public enum FlameState { High, Medium, Low, Extinguished }
public class FlameManager : MonoBehaviour
{
    [Header("Flame Settings")]
    [SerializeField, Tooltip("Máximo de chama")] private float maxFlame;

    [SerializeField, Tooltip("Velocidade que a chama diminui")] private float decayRate = 2f;
    private float currentFlame;

    public FlameState CurrentFlameState { get; private set; }

    //Events
    public event Action OnFlameExtinguished;
    public event Action<FlameState> OnFlameStateChanged;

    public float MaxFlame => maxFlame;
    public float CurrentFlame => currentFlame;

    void Start()
    {
        currentFlame = maxFlame;
        UpdateFlameState();
    }

    void Update()
    {
        if (CurrentFlameState == FlameState.Extinguished) return;

        ConsumeFlame();
        UpdateFlameState();

        //Debug.Log(currentFlame);
    }

    void UpdateFlameState()
    {
        float flamePercentage = currentFlame / maxFlame;
        FlameState NewState;

        if (currentFlame <= 0) NewState = FlameState.Extinguished;
        else if (flamePercentage <= 0.25f) NewState = FlameState.Low;
        else if (flamePercentage <= 0.5f) NewState = FlameState.Medium;
        else NewState = FlameState.High;

        if (NewState != CurrentFlameState)
        {
            CurrentFlameState = NewState;
            OnFlameStateChanged?.Invoke(CurrentFlameState);
            Debug.Log("Mudança de estado da chama: " + CurrentFlameState);

            if (CurrentFlameState == FlameState.Extinguished)
            {
                OnFlameExtinguished?.Invoke();
                Debug.Log("Chama esgotada :(");
            }
        }

    }

    public void AddFlame(float amount)
    {
        if (CurrentFlameState == FlameState.Extinguished) return;
        if (currentFlame >= maxFlame) return;

        currentFlame += amount;
        currentFlame = Mathf.Clamp(currentFlame, 0, maxFlame);
        UpdateFlameState();
    }

    void ConsumeFlame()
    {
        currentFlame -= Time.deltaTime * decayRate;
        currentFlame = Mathf.Clamp(currentFlame, 0, maxFlame);
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Candle"))
        {
            //AddFlame();
            Debug.Log("Achou uma vela! Chama atual: " + currentFlame);
        }
    }*/
}

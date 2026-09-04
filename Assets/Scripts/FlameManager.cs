using System;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public enum FlameState { High, Medium, Low, Extinguished }
public class FlameManager : MonoBehaviour
{
    [SerializeField] private float currentFlame;
    [SerializeField] private float maxFlame;

    [SerializeField] private float candleFlame;

    public FlameState CurrentFlameState { get; private set; }

    //Events
    public event Action OnFlameExtinguished;
    public event Action<FlameState> OnFlameStateChanged;

    void Start()
    {
        currentFlame = maxFlame;
        UpdateFlameState();
    }

    void Update()
    {
        if (CurrentFlameState == FlameState.Extinguished) return;

        LosingFlame();
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

    void LosingFlame()
    {
        currentFlame -= Time.deltaTime;
        currentFlame = Mathf.Clamp(currentFlame, 0, maxFlame);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Candle"))
        {
            currentFlame += candleFlame;
            currentFlame = Mathf.Clamp(currentFlame, 0, maxFlame);
            Debug.Log("Achou uma vela! Chama atual: " + currentFlame);
        }
    }
}

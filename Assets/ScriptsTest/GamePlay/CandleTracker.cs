using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(FlameManagerAdapter))]
public sealed class CandleTracker : MonoBehaviour
{
    [SerializeField] private GameplayManager gameplayManager;

    private void Awake()
    {
        if (gameplayManager == null)
        {
            gameplayManager =
                FindFirstObjectByType<GameplayManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Candle candle =
            other.GetComponentInParent<Candle>();

        if (candle == null || gameplayManager == null)
            return;

        gameplayManager.RegisterCandleCollected(candle);
    }
}
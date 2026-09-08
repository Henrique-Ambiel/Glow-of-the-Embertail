using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Candle))]
public sealed class CandleCheckpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private Candle candle;
    private GameplayManager gameplayManager;
    private bool activated;

    private void Awake()
    {
        candle = GetComponent<Candle>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (activated)
            return;

        FlameManagerAdapter playerFlame =
            other.GetComponentInParent<FlameManagerAdapter>();

        if (playerFlame == null)
            return;

        if (gameplayManager == null)
        {
            gameplayManager =
                FindFirstObjectByType<GameplayManager>();
        }

        if (gameplayManager == null)
        {
            Debug.LogWarning(
                "Nenhum GameplayManager foi encontrado na cena.",
                this
            );

            return;
        }

        activated = true;

        Transform checkpoint =
            respawnPoint != null
                ? respawnPoint
                : transform;

        gameplayManager.ActivateCheckpoint(
            candle,
            checkpoint
        );
    }
}
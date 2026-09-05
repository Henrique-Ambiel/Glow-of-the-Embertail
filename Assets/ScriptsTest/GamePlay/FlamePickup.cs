using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class FlamePickup : MonoBehaviour
{
    [SerializeField] private GameplayManager gameplayManager;

    [Header("Recompensa")]
    [SerializeField, Min(0)] private int points = 100;
    [SerializeField, Min(0f)] private float flameRestore = 10f;

    [Header("Visual")]
    [SerializeField] private GameObject visualToDisable;

    private bool collected;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();

        if (gameplayManager == null)
        {
            gameplayManager =
                FindFirstObjectByType<GameplayManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        FlameManagerAdapter flameEnergy =
            other.GetComponentInParent<FlameManagerAdapter>();

        if (flameEnergy == null)
            return;

        collected = true;

        flameEnergy.Restore(flameRestore);

        if (gameplayManager != null)
            gameplayManager.AddPoints(points);

        if (visualToDisable != null)
            visualToDisable.SetActive(false);

        triggerCollider.enabled = false;
    }

    private void Reset()
    {
        Collider currentCollider = GetComponent<Collider>();

        if (currentCollider != null)
            currentCollider.isTrigger = true;
    }
}
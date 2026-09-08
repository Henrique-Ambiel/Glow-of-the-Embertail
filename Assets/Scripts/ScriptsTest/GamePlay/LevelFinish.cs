using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class LevelFinish : MonoBehaviour
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
        if (
            other.GetComponentInParent<FlameManagerAdapter>()
            == null
        )
        {
            return;
        }

        if (gameplayManager != null)
            gameplayManager.FinishLevel();
    }

    private void Reset()
    {
        Collider currentCollider = GetComponent<Collider>();

        if (currentCollider != null)
            currentCollider.isTrigger = true;
    }
}
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private GameplayManager gameplayManager;
    private bool activated;

    private void Awake()
    {
        gameplayManager =
            FindFirstObjectByType<GameplayManager>();

        if (respawnPoint == null)
            respawnPoint = transform.Find("RespawnPoint");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        FlameManagerAdapter player =
            other.GetComponentInParent<FlameManagerAdapter>();

        if (player == null)
            return;

        if (gameplayManager == null)
        {
            gameplayManager =
                FindFirstObjectByType<GameplayManager>();
        }

        if (gameplayManager == null || respawnPoint == null)
        {
            Debug.LogWarning(
                "GameplayManager ou RespawnPoint não configurado.",
                this
            );

            return;
        }

        gameplayManager.ActivateCheckpoint(respawnPoint);
        activated = true;
    }

    private void Reset()
    {
        Collider checkpointCollider = GetComponent<Collider>();
        checkpointCollider.isTrigger = true;
    }

    private void OnDrawGizmosSelected()
    {
        Transform point = respawnPoint;

        if (point == null)
            point = transform.Find("RespawnPoint");

        if (point == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(point.position, 0.25f);
        Gizmos.DrawLine(transform.position, point.position);
    }
}
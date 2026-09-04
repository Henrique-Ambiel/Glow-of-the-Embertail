using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerDash : MonoBehaviour
{
    [Header("Trajeto")]
    [SerializeField, Min(0.01f)] private float distance = 4.5f;
    [SerializeField, Min(0.01f)] private float duration = 0.18f;
    [SerializeField, Min(0f)] private float cooldown = 0.12f;

    [Header("Física")]
    [SerializeField] private float gravity = -25f;

    [Header("Energia")]
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField, Min(0f)] private float baseEnergyCost = 10f;
    [SerializeField, Min(0f)] private float consecutiveCostIncrease = 5f;
    [SerializeField, Min(0f)] private float consecutiveWindow = 1.2f;

    private float currentDashCost;
    private float energySpentDuringDash;
    private int consecutiveDashes;
    private double lastDashStartedAt = double.NegativeInfinity;

    private CharacterController controller;
    private Animator animator;
    private Vector3 direction;
    private float elapsed;
    private double readyAt;
    private float verticalVelocity;
    private float planeZ;

    public bool IsDashing { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentsInChildren<Animator>()[0];
        
        if (playerEnergy == null)
        {
                playerEnergy = GetComponent<PlayerEnergy>();
        }
        
        planeZ = transform.position.z;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsDashing && Time.timeAsDouble >= readyAt)
        {
            HandleMouseDash();
        }

        ProcessMovement(Time.deltaTime);
        LockZPlane();
    }

    private void HandleMouseDash()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Plane gameplayPlane = new Plane(Vector3.forward, transform.position);
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        if (gameplayPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            Vector2 aimDirection = new Vector2(targetPoint.x - transform.position.x, targetPoint.y - transform.position.y).normalized;
            
            if (aimDirection.sqrMagnitude > 0.0001f)
            {
                direction = new Vector3(aimDirection.x, aimDirection.y, 0f).normalized;
                elapsed = 0f;
                energySpentDuringDash = 0f;
                double now = Time.timeAsDouble;
                consecutiveDashes = now - lastDashStartedAt < consecutiveWindow
                    ? consecutiveDashes + 1
                    : 1;
                lastDashStartedAt = now;
                currentDashCost = baseEnergyCost
                    + (consecutiveDashes - 1) * consecutiveCostIncrease;
                FaceDashDirection();
                IsDashing = true;
                animator.SetBool("Dash", true);
            }
        }
    }

    private void FaceDashDirection()
    {
        if (Mathf.Abs(direction.x) < 0.001f)
        {
            return;
        }

        Vector3 horizontalDirection =
            Vector3.right * Mathf.Sign(direction.x);

        transform.rotation = Quaternion.LookRotation(
            horizontalDirection,
            Vector3.up
        );
    }

    private void ProcessMovement(float deltaTime)
    {
        if (deltaTime <= 0f)
        {
            return;
        }

        if (IsDashing)
        {
            float step = Mathf.Min(
                deltaTime,
                duration - elapsed
            );

            elapsed += step;

            // Distribui o custo ao longo do dash.
            float targetCost =
                currentDashCost
                * Mathf.Clamp01(elapsed / duration);

            float frameCost =
                targetCost - energySpentDuringDash;

            energySpentDuringDash = targetCost;

            if (playerEnergy != null)
            {
                playerEnergy.Consume(frameCost);

                if (playerEnergy.IsDepleted)
                {
                    FinishDash();
                    return;
                }
            }

            Vector3 displacement =
                direction
                * (distance / duration)
                * step;

            CollisionFlags collision =
                controller.Move(displacement);

            bool hitSide =
                (collision & CollisionFlags.Sides) != 0
                && Mathf.Abs(direction.x) > 0.001f;

            bool hitCeiling =
                (collision & CollisionFlags.Above) != 0
                && direction.y > 0.001f;

            bool hitGround =
                (collision & CollisionFlags.Below) != 0
                && direction.y < -0.001f;

            if (
                hitSide
                || hitCeiling
                || hitGround
                || elapsed >= duration - 0.000001f
            )
            {
                FinishDash();
            }

            return;
        }

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * deltaTime;

        controller.Move(
            Vector3.up * (verticalVelocity * deltaTime)
        );
    }

    private void LockZPlane()
    {
        Vector3 position = transform.position;
        position.z = planeZ;
        transform.position = position;
    }

    private void FinishDash()
    {
        if (!IsDashing)
        {
            return;
        }

        IsDashing = false;
        animator.SetBool("Dash", false);
        verticalVelocity = -2f;
        readyAt = Time.timeAsDouble + cooldown;
    }
}
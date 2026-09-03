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

    private CharacterController controller;
    private Vector3 direction;
    private float elapsed;
    private double readyAt;
    private float verticalVelocity;
    private float planeZ;

    public bool IsDashing { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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
                direction = new Vector3(aimDirection.x, aimDirection.y, 0f);
                elapsed = 0f;
                IsDashing = true;
            }
        }
    }

    private void ProcessMovement(float deltaTime)
    {
        if (deltaTime <= 0f) return;

        Vector3 frameVelocity = Vector3.zero;

        if (IsDashing)
        {
            float step = Mathf.Min(deltaTime, duration - elapsed);
            elapsed += step;
            
            frameVelocity = direction * (distance / duration);
            verticalVelocity = 0f; 

            if (elapsed >= duration - 0.000001f)
            {
                IsDashing = false;
                readyAt = Time.timeAsDouble + cooldown;
            }
        }
        else
        {
            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f; 
            }
            verticalVelocity += gravity * deltaTime;
            frameVelocity.y = verticalVelocity;
        }

        CollisionFlags collision = controller.Move(frameVelocity * deltaTime);

        if(IsDashing && (collision & CollisionFlags.Sides) != 0)
        {
            IsDashing = false;
            verticalVelocity = -2f;
            readyAt = Time.timeAsDouble + cooldown;
        }
    }

    private void LockZPlane()
    {
        Vector3 position = transform.position;
        position.z = planeZ;
        transform.position = position;
    }
}
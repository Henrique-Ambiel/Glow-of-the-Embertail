using UnityEngine;

[DisallowMultipleComponent]
public sealed class TailFlameController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FlameManagerAdapter flameEnergy;
    [SerializeField] private Renderer flameRenderer;
    [SerializeField] private Transform flameScaleRoot;
    [SerializeField] private Light flameLight;

    [Header("Orientacao")]
    [SerializeField] private Transform flameBillboard;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool faceCamera = true;

    [Header("Resposta visual")]
    [SerializeField, Min(0.01f)] private float responseSpeed = 6f;
    [SerializeField, Range(0.05f, 1f)] private float minimumWidth = 0.55f;
    [SerializeField, Range(0.05f, 1f)] private float minimumHeight = 0.30f;

    [Header("Luz opcional")]
    [SerializeField, Min(0f)] private float maximumLightIntensity = 3f;
    [SerializeField, Min(0f)] private float maximumLightRange = 2.5f;

    private static readonly int FlameAmountId =
        Shader.PropertyToID("_FlameAmount");

    private MaterialPropertyBlock propertyBlock;
    private Vector3 originalScale;
    private float targetAmount;
    private float displayedAmount;

    private void Awake()
    {
        if (flameEnergy == null)
            flameEnergy = GetComponentInParent<FlameManagerAdapter>();

        if (flameRenderer == null)
            flameRenderer = GetComponentInChildren<Renderer>(true);

        if (flameScaleRoot == null)
            flameScaleRoot = transform;

        if (flameBillboard == null && flameRenderer != null)
            flameBillboard = flameRenderer.transform;

        originalScale = flameScaleRoot.localScale;
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (flameEnergy == null)
        {
            Debug.LogError(
                "TailFlameController nao encontrou o FlameManagerAdapter.",
                this
            );

            enabled = false;
            return;
        }

        if (targetCamera == null)
            targetCamera = Camera.main;

        SetTarget(flameEnergy.Current, flameEnergy.Maximum);
        displayedAmount = targetAmount;
        ApplyVisual();
    }

    private void LateUpdate()
    {
        if (!faceCamera || flameBillboard == null)
            return;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        Vector3 cameraDirection =
            targetCamera.transform.position - flameBillboard.position;
        cameraDirection = Vector3.ProjectOnPlane(
            cameraDirection,
            Vector3.up
        );

        if (cameraDirection.sqrMagnitude < 0.0001f)
            return;

        flameBillboard.rotation = Quaternion.LookRotation(
            cameraDirection.normalized,
            Vector3.up
        );
    }

    private void Update()
    {
        // Le o valor real em todos os frames. Isso acompanha tanto o
        // consumo quanto qualquer recarga feita diretamente no FlameManager.
        if (flameEnergy != null)
            targetAmount = Mathf.Clamp01(flameEnergy.Normalized);

        displayedAmount = Mathf.MoveTowards(
            displayedAmount,
            targetAmount,
            responseSpeed * Time.deltaTime
        );

        ApplyVisual();
    }

    private void SetTarget(float current, float maximum)
    {
        targetAmount = maximum > 0f
            ? Mathf.Clamp01(current / maximum)
            : 0f;
    }

    private void ApplyVisual()
    {
        bool hasFlame = displayedAmount > 0.001f;

        if (flameRenderer != null)
        {
            flameRenderer.enabled = hasFlame;
            flameRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(FlameAmountId, displayedAmount);
            flameRenderer.SetPropertyBlock(propertyBlock);
        }

        float width = Mathf.Lerp(
            minimumWidth,
            1f,
            displayedAmount
        );
        float height = Mathf.Lerp(
            minimumHeight,
            1f,
            displayedAmount
        );

        flameScaleRoot.localScale = new Vector3(
            originalScale.x * width,
            originalScale.y * height,
            originalScale.z * width
        );

        if (flameLight != null)
        {
            flameLight.enabled = hasFlame;
            flameLight.intensity =
                maximumLightIntensity * displayedAmount;
            flameLight.range = Mathf.Lerp(
                maximumLightRange * 0.35f,
                maximumLightRange,
                displayedAmount
            );
        }
    }
}

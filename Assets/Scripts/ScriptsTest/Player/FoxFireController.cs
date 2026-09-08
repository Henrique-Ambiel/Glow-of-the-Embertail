using UnityEngine;

[DisallowMultipleComponent]
public sealed class FoxFireController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FlameManagerAdapter flameEnergy;
    [SerializeField] private SkinnedMeshRenderer[] foxRenderers;
    [SerializeField] private Light foxLight;

    [Header("Resposta visual")]
    [SerializeField, Min(0.01f)] private float responseSpeed = 4f;

    [Header("Luz opcional")]
    [SerializeField, Min(0f)] private float maximumLightIntensity = 4f;
    [SerializeField, Min(0f)] private float maximumLightRange = 3f;

    private static readonly int FlameAmountId =
        Shader.PropertyToID("_FlameAmount");

    private MaterialPropertyBlock propertyBlock;
    private float displayedAmount;

    private void Awake()
    {
        if (flameEnergy == null)
            flameEnergy = GetComponent<FlameManagerAdapter>();

        if (foxRenderers == null || foxRenderers.Length == 0)
            foxRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (flameEnergy == null)
        {
            Debug.LogError(
                "FoxFireController nao encontrou o FlameManagerAdapter.",
                this
            );
            enabled = false;
            return;
        }

        displayedAmount = Mathf.Clamp01(flameEnergy.Normalized);
        ApplyVisual();
    }

    private void Update()
    {
        float targetAmount = Mathf.Clamp01(flameEnergy.Normalized);

        displayedAmount = Mathf.MoveTowards(
            displayedAmount,
            targetAmount,
            responseSpeed * Time.deltaTime
        );

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        foreach (SkinnedMeshRenderer foxRenderer in foxRenderers)
        {
            if (foxRenderer == null)
                continue;

            foxRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(FlameAmountId, displayedAmount);
            foxRenderer.SetPropertyBlock(propertyBlock);
        }

        if (foxLight != null)
        {
            foxLight.enabled = displayedAmount > 0.001f;
            foxLight.intensity =
                maximumLightIntensity * displayedAmount;
            foxLight.range = Mathf.Lerp(
                maximumLightRange * 0.35f,
                maximumLightRange,
                displayedAmount
            );
        }
    }
}

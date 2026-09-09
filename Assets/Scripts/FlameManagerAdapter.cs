using System;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(FlameManager))]
public sealed class FlameManagerAdapter : MonoBehaviour
{
    [SerializeField] private FlameManager flameManager;

    private FieldInfo currentFlameField;
    private FieldInfo maxFlameField;
    private MethodInfo updateFlameStateMethod;

    private float lastCurrent;
    private float lastMaximum;
    private bool isReady;

    public event Action<float, float> FlameChanged;
    public event Action<float> FlameSpent;
    public event Action<FlameState> StateChanged;
    public event Action Extinguished;

    public float Current =>
        isReady ? ReadField(currentFlameField) : 0f;

    public float Maximum =>
        isReady ? ReadField(maxFlameField) : 0f;

    public float Normalized =>
        Maximum > 0f ? Current / Maximum : 0f;

    public bool IsExtinguished => Current <= 0f;

    public FlameState CurrentState =>
        flameManager != null
            ? flameManager.CurrentFlameState
            : FlameState.Extinguished;

    private void Awake()
    {
        if (flameManager == null)
            flameManager = GetComponent<FlameManager>();

        if (flameManager == null)
        {
            Debug.LogError(
                "FlameManagerAdapter não encontrou o FlameManager.",
                this
            );

            enabled = false;
            return;
        }

        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.NonPublic;

        Type flameManagerType = flameManager.GetType();

        currentFlameField = flameManagerType.GetField(
            "currentFlame",
            flags
        );

        maxFlameField = flameManagerType.GetField(
            "maxFlame",
            flags
        );

        updateFlameStateMethod = flameManagerType.GetMethod(
            "UpdateFlameState",
            flags
        );

        isReady =
            currentFlameField != null &&
            maxFlameField != null &&
            updateFlameStateMethod != null;

        if (!isReady)
        {
            Debug.LogError(
                "O FlameManager foi alterado e o adaptador não encontrou " +
                "currentFlame, maxFlame ou UpdateFlameState.",
                this
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (flameManager == null)
            return;

        flameManager.OnFlameStateChanged += HandleStateChanged;
        flameManager.OnFlameExtinguished += HandleExtinguished;
    }

    private void Start()
    {
        NotifyFlameChanged(true);
    }

    private void LateUpdate()
    {
        // Detecta a drenagem passiva e a recuperação pelas velas.
        NotifyFlameChanged(false);
    }

    private void OnDisable()
    {
        if (flameManager == null)
            return;

        flameManager.OnFlameStateChanged -= HandleStateChanged;
        flameManager.OnFlameExtinguished -= HandleExtinguished;
    }

    public float Consume(float amount)
    {
        if (!isReady || amount <= 0f || IsExtinguished)
            return 0f;

        float previous = Current;
        float updated = Mathf.Max(0f, previous - amount);
        float consumed = previous - updated;

        SetCurrent(updated);

        if (consumed > 0f)
            FlameSpent?.Invoke(consumed);

        return consumed;
    }

    public float Restore(float amount)
    {
        if (!isReady || amount <= 0f)
            return 0f;

        float previous = Current;
        float updated = Mathf.Min(Maximum, previous + amount);
        float restored = updated - previous;

        SetCurrent(updated);
        return restored;
    }

    private void SetCurrent(float value)
    {
        currentFlameField.SetValue(
            flameManager,
            Mathf.Clamp(value, 0f, Maximum)
        );

        // Atualiza os estados High, Medium, Low e Extinguished.
        updateFlameStateMethod.Invoke(flameManager, null);

        NotifyFlameChanged(false);
    }

    private float ReadField(FieldInfo field)
    {
        return (float)field.GetValue(flameManager);
    }

    private void NotifyFlameChanged(bool force)
    {
        if (!isReady)
            return;

        float current = Current;
        float maximum = Maximum;

        if (
            force ||
            !Mathf.Approximately(current, lastCurrent) ||
            !Mathf.Approximately(maximum, lastMaximum)
        )
        {
            lastCurrent = current;
            lastMaximum = maximum;

            FlameChanged?.Invoke(current, maximum);
        }
    }

    private void HandleStateChanged(FlameState state)
    {
        StateChanged?.Invoke(state);
    }

    private void HandleExtinguished()
    {
        Extinguished?.Invoke();
    }
}
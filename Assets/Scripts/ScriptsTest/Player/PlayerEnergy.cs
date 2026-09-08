using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerEnergy : MonoBehaviour
{
        [Header("Energia / Vida")]
        [SerializeField, Min(1f)] private float maximum = 100f;
        [SerializeField, Min(0f)] private float initial = 100f;

        [Header("Drenagem fora das velas")]
        [SerializeField, Min(0f)] private float passiveDrainPerSecond = 3f;
        [SerializeField] private bool passiveDrainEnabled = true;

        private readonly HashSet<UnityEngine.Object> protectionSources = new();

        public event Action<float, float> EnergyChanged;
        public event Action<float> EnergySpent;
        public event Action EnergyDepleted;

        public float Current { get; private set; }
        public float Maximum => maximum;
        public float Normalized => maximum > 0f ? Current / maximum : 0f;
        public bool IsDepleted => Current <= 0f;
        public bool IsProtected => protectionSources.Count > 0;

        private void Awake()
        {
            Current = Mathf.Clamp(initial, 0f, maximum);

        }

        private void Start() => NotifyChanged();

        private void Update() => Tick(Time.deltaTime);

        internal void Tick(float deltaTime)
        {
            if (!passiveDrainEnabled || IsProtected || IsDepleted || deltaTime <= 0f)
                return;

            Consume(passiveDrainPerSecond * deltaTime);
        }

        public float Consume(float amount)
        {
            if (amount <= 0f || IsDepleted)
                return 0f;

            float previous = Current;
            Current = Mathf.Max(0f, Current - amount);
            float consumed = previous - Current;

            EnergySpent?.Invoke(consumed);
            NotifyChanged();

            if (Current <= 0f)
                EnergyDepleted?.Invoke();

            return consumed;
        }

        public float Restore(float amount)
        {
            if (amount <= 0f || Current >= maximum)
                return 0f;

            float previous = Current;
            Current = Mathf.Min(maximum, Current + amount);
            NotifyChanged();
            return Current - previous;
        }

        public void ResetEnergy()
        {
            Current = maximum;
            NotifyChanged();
        }

        public void SetEnergy(float amount)
        {
            bool wasAlive = !IsDepleted;
            Current = Mathf.Clamp(amount, 0f, maximum);
            NotifyChanged();

            if (wasAlive && IsDepleted)
                EnergyDepleted?.Invoke();
        }

        public void SetProtected(UnityEngine.Object source, bool protectedBySource)
        {
            if (source == null)
                return;

            if (protectedBySource)
                protectionSources.Add(source);
            else
                protectionSources.Remove(source);
        }

        private void NotifyChanged() => EnergyChanged?.Invoke(Current, maximum);

        private void OnValidate()
        {
            maximum = Mathf.Max(1f, maximum);
            initial = Mathf.Clamp(initial, 0f, maximum);
            passiveDrainPerSecond = Mathf.Max(0f, passiveDrainPerSecond);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Water"))
                Consume(Maximum);
        }
}

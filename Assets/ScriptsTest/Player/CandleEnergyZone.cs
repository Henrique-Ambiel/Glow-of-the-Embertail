using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class CandleEnergyZone : MonoBehaviour
{
        [SerializeField, Min(0f)] private float restorationPerSecond;

        private readonly HashSet<PlayerEnergy> protectedPlayers = new();

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            if (restorationPerSecond <= 0f)
                return;

            foreach (PlayerEnergy energy in protectedPlayers)
                if (energy != null)
                    energy.Restore(restorationPerSecond * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerEnergy energy = other.GetComponentInParent<PlayerEnergy>();
            if (energy == null || !protectedPlayers.Add(energy))
                return;

            energy.SetProtected(this, true);
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerEnergy energy = other.GetComponentInParent<PlayerEnergy>();
            if (energy == null || !protectedPlayers.Remove(energy))
                return;

            energy.SetProtected(this, false);
        }

        private void OnDisable()
        {
            foreach (PlayerEnergy energy in protectedPlayers)
                if (energy != null)
                    energy.SetProtected(this, false);

            protectedPlayers.Clear();
        }

        private void OnValidate()
        {
            restorationPerSecond = Mathf.Max(0f, restorationPerSecond);
            Collider trigger = GetComponent<Collider>();
            if (trigger != null)
                trigger.isTrigger = true;
        }
}

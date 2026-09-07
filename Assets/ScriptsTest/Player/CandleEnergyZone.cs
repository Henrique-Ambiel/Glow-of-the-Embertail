using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class CandleEnergyZone : MonoBehaviour
{
        [SerializeField, Min(0f)] private float restorationPerSecond;

        private readonly HashSet<FlameManager> protectedPlayers = new();

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            if (restorationPerSecond <= 0f)
                return;

            foreach (FlameManager energy in protectedPlayers)
                if (energy != null)
                    energy.AddFlame(restorationPerSecond * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            FlameManager energy = other.GetComponentInParent<FlameManager>();
            if (energy == null || !protectedPlayers.Add(energy))
                return;

            //energy.SetProtected(this, true);
        }

        private void OnTriggerExit(Collider other)
        {
            FlameManager energy = other.GetComponentInParent<FlameManager>();
            if (energy == null || !protectedPlayers.Remove(energy))
                return;

            //energy.SetProtected(this, false);
        }

        private void OnDisable()
        {
            //foreach (FlameManager energy in protectedPlayers)
                //if (energy != null)
                    //energy.SetProtected(this, false);

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

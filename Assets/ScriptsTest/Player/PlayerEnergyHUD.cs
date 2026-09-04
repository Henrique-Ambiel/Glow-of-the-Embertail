using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PlayerEnergyHUD : MonoBehaviour
{
        [SerializeField] private PlayerEnergy playerEnergy;
        [SerializeField] private Slider energyBar;
        [SerializeField] private TMP_Text energyText;

        private void OnEnable()
        {
            if (playerEnergy == null)
                return;

            playerEnergy.EnergyChanged += Refresh;
            Refresh(playerEnergy.Current, playerEnergy.Maximum);
        }

        private void OnDisable()
        {
            if (playerEnergy != null)
                playerEnergy.EnergyChanged -= Refresh;
        }

        public void SetPlayer(PlayerEnergy energy)
        {
            if (isActiveAndEnabled && playerEnergy != null)
                playerEnergy.EnergyChanged -= Refresh;

            playerEnergy = energy;

            if (isActiveAndEnabled && playerEnergy != null)
            {
                playerEnergy.EnergyChanged += Refresh;
                Refresh(playerEnergy.Current, playerEnergy.Maximum);
            }
        }

        private void Refresh(float current, float maximum)
        {
            if (energyBar != null)
            {
                energyBar.minValue = 0f;
                energyBar.maxValue = maximum;
                energyBar.value = current;
            }

            if (energyText != null)
                energyText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(maximum)}";
        }
}

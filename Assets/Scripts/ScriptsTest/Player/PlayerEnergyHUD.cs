using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PlayerEnergyHUD : MonoBehaviour
{
    [SerializeField] private FlameManagerAdapter flameEnergy;
    [SerializeField] private Slider energyBar;
    [SerializeField] private TMP_Text energyText;

    private void OnEnable()
    {
        if (flameEnergy == null)
            return;

        flameEnergy.FlameChanged += Refresh;

        Refresh(
            flameEnergy.Current,
            flameEnergy.Maximum
        );
    }

    private void OnDisable()
    {
        if (flameEnergy != null)
            flameEnergy.FlameChanged -= Refresh;
    }

    public void SetFlameManager(
        FlameManagerAdapter adapter
    )
    {
        if (isActiveAndEnabled && flameEnergy != null)
            flameEnergy.FlameChanged -= Refresh;

        flameEnergy = adapter;

        if (isActiveAndEnabled && flameEnergy != null)
        {
            flameEnergy.FlameChanged += Refresh;

            Refresh(
                flameEnergy.Current,
                flameEnergy.Maximum
            );
        }
    }

    private void Refresh(
        float current,
        float maximum
    )
    {
        if (energyBar != null)
        {
            energyBar.minValue = 0f;
            energyBar.maxValue = maximum;
            energyBar.value = current;
        }

        if (energyText != null)
        {
            energyText.text =
                $"{Mathf.CeilToInt(current)} / " +
                $"{Mathf.CeilToInt(maximum)}";
        }
    }
}
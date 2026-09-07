using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private SoundType hover = SoundType.Hover;
    [SerializeField] private SoundType click = SoundType.Click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundEvents.Instance.PlaySFX(hover);
    }

    public void PlayClickSFX()
    {
        SoundEvents.Instance.PlaySFX(click);
    }
}

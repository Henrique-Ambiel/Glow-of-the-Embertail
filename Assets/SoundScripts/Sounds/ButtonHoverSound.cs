using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private SoundEventsData soundEvents;
    [SerializeField] private SoundType hover = SoundType.Hover;
    [SerializeField] private SoundType click = SoundType.Click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        soundEvents.PlaySFX(hover);
    }

    public void PlayClickSFX()
    {
        soundEvents.PlaySFX(click);
    }
}

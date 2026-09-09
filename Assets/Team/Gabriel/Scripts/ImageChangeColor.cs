using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageChangeColor : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image image;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color pressedColor;
    //[SerializeField] private Color selectedColor;
    //[SerializeField] private Color disabledColor;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = pressedColor;
    }
}

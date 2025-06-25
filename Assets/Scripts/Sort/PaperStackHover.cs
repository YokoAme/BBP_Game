using UnityEngine;
using UnityEngine.EventSystems;

public class PaperStackHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject glowLayer;

    public void OnPointerEnter(PointerEventData eventData)
    {
        glowLayer.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        glowLayer.SetActive(false);
    }
}

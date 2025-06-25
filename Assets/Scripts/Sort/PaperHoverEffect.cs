using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PaperHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Глобальный флаг: true, пока тянем ЛЮБУЮ карточку
    public static bool draggingAny = false;

    [SerializeField] Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    [SerializeField] float speed = 8f;

    Vector3 originalScale;
    Coroutine tween;

    void Awake() => originalScale = transform.localScale;

    public void ApplyHoverNow()
    {
        if (tween != null) StopCoroutine(tween);
        transform.localScale = hoverScale;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (draggingAny) return;
        StartTween(hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggingAny) return;
        StartTween(originalScale);
    }

    // Вызываем из PaperDrag, чтобы сразу вернуть масштаб без анимации
    public void ResetImmediate()
    {
        if (tween != null) StopCoroutine(tween);
        transform.localScale = originalScale;
    }

    void StartTween(Vector3 target)
    {
        if (tween != null) StopCoroutine(tween);
        tween = StartCoroutine(ScaleTo(target));
    }

    IEnumerator ScaleTo(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
            yield return null;
        }
        transform.localScale = target;
    }
}

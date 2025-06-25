using UnityEngine;
using System.Collections;

public class SlideInUI : MonoBehaviour
{
    [Tooltip("Сколько пикселей элемент стартует за экраном")]
    public float offsetX = 400f;

    [Tooltip("Длительность анимации (в секундах)")]
    public float duration = 0.4f;

    [Tooltip("Плавность (EaseInOut по умолчанию)")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rt;
    private Vector2 targetPos;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        targetPos = rt.anchoredPosition;
        rt.anchoredPosition = targetPos + Vector2.right * offsetX; // смещаем вправо
    }

    public void Play()
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = curve.Evaluate(t / duration);
            rt.anchoredPosition = Vector2.LerpUnclamped(targetPos + Vector2.right * offsetX, targetPos, k);
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }
}

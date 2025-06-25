using UnityEngine;
using System.Collections;

public class SpriteEnter : MonoBehaviour
{
    public float duration = 0.5f;
    public Vector2 offscreenOffset = new Vector2(-1000, 0); // смещение (по умолчанию - слева)
    public bool fromTransparency = false;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void EnterFromLeft()
    {
        StartCoroutine(AnimateFrom(rect.anchoredPosition + offscreenOffset));
    }

    public void EnterFromRight()
    {
        Vector2 offset = new Vector2(Mathf.Abs(offscreenOffset.x), 0);
        StartCoroutine(AnimateFrom(rect.anchoredPosition + offset));
    }

    public void EnterFromBottom()
    {
        Vector2 offset = new Vector2(0, -Mathf.Abs(offscreenOffset.x));
        StartCoroutine(AnimateFrom(rect.anchoredPosition + offset));
    }

    public void EnterFadeOnly()
    {
        StartCoroutine(FadeOnly());
    }

    IEnumerator AnimateFrom(Vector2 startPos)
    {
        Vector2 endPos = rect.anchoredPosition;
        rect.anchoredPosition = startPos;

        if (fromTransparency)
            canvasGroup.alpha = 0;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);

            if (fromTransparency)
                canvasGroup.alpha = progress;

            yield return null;
        }

        rect.anchoredPosition = endPos;
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOnly()
    {
        canvasGroup.alpha = 0;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public void ExitToLeft()
    {
        StartCoroutine(AnimateTo(rect.anchoredPosition + offscreenOffset));
    }

    public void ExitToRight()
    {
        Vector2 offset = new Vector2(Mathf.Abs(offscreenOffset.x), 0);
        StartCoroutine(AnimateTo(rect.anchoredPosition + offset));
    }

    public void ExitToBottom()
    {
        Vector2 offset = new Vector2(0, -Mathf.Abs(offscreenOffset.x));
        StartCoroutine(AnimateTo(rect.anchoredPosition + offset));
    }

    IEnumerator AnimateTo(Vector2 endPos)
    {
        Vector2 startPos = rect.anchoredPosition;

        if (fromTransparency)
            canvasGroup.alpha = 1;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);

            if (fromTransparency)
                canvasGroup.alpha = 1 - progress;

            yield return null;
        }

        rect.anchoredPosition = endPos;
        canvasGroup.alpha = fromTransparency ? 0 : 1;
        gameObject.SetActive(false); // ѕр€чем после ухода
    }

}

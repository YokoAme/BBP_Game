using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PaperDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    Canvas canvas;
    RectTransform rt;
    CanvasGroup cg;


    Vector3 originalScale;
    static readonly Vector3 dragScale = new Vector3(1.1f, 1.1f, 1f);

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        cg = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PaperHoverEffect.draggingAny = true;

        var hover = GetComponent<PaperHoverEffect>();
        if (hover != null) hover.ResetImmediate();

        transform.localScale = dragScale;
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PaperHoverEffect.draggingAny = false;
        cg.blocksRaycasts = true;

        bool cursorStillOver = RectTransformUtility.RectangleContainsScreenPoint(
            rt, Input.mousePosition, canvas.worldCamera);

        var hover = GetComponent<PaperHoverEffect>();
        if (cursorStillOver && hover != null)
            hover.ApplyHoverNow();
        else
            transform.localScale = originalScale;

        if (eventData.pointerEnter != null)
        {
            FolderHighlighter folder = eventData.pointerEnter.GetComponentInParent<FolderHighlighter>();
            if (folder != null)
            {
                var myMeta = GetComponent<CardMeta>();
                bool isCorrect = folder.folderType == myMeta.correctFolder;

                ScoreManager.Instance.RegisterResult(isCorrect);

                folder.OnPaperDropped();

                cg.blocksRaycasts = false;
                cg.interactable = false;
                StartCoroutine(FlyToFolder(folder.transform));
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    IEnumerator FlyToFolder(Transform target)
    {
        float time = 0f;
        float duration = 0.25f;

        RectTransform myRT = GetComponent<RectTransform>();
        RectTransform targetRT = target.GetComponent<RectTransform>();

        Vector2 startPos = myRT.anchoredPosition;
        Vector2 endPos = targetRT.anchoredPosition;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        while (time < duration)
        {
            time += Time.deltaTime;
            myRT.anchoredPosition = Vector2.Lerp(startPos, endPos, time / duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            yield return null;
        }

        GetComponent<PaperCard>()?.DestroyCard();
    }


}

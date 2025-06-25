using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class IngredientSource : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject copyPrefab;
    public Sprite iconSprite;
    public string ingredientType;

    private Canvas canvas;
    private GameObject copy;
    private RectTransform copyRT;
    private Vector3 spawnPoint;

    private ShakerZone shakerZone;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        shakerZone = FindObjectOfType<ShakerZone>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        // Если копии нет — создаём
        if (copy == null)
        {
            copy = Instantiate(copyPrefab, canvas.transform);
            copyRT = copy.GetComponent<RectTransform>();
            copy.AddComponent<DraggedIngredient>().type = ingredientType;
            copy.GetComponent<Image>().sprite = iconSprite;
        }

        // Активируем, ставим на мышку
        copy.SetActive(true);
        copyRT.position = e.position;
        spawnPoint = e.position;
    }

    public void OnDrag(PointerEventData e)
    {
        if (copyRT != null)
            copyRT.position = e.position;
    }

    public void OnEndDrag(PointerEventData _)
    {
        if (copy == null) return;

        if (IsOverlapping(copyRT, shakerZone.GetComponent<RectTransform>()))
        {
            string type = copy.GetComponent<DraggedIngredient>().type;
            shakerZone.TryAdd(type);
            StartCoroutine(SuckIntoShaker()); // вот она, анимация
        }
        else
        {
            StartCoroutine(ReturnAndHide());
        }
    }

    IEnumerator SuckIntoShaker()
    {
        Vector3 startPos = copyRT.position;
        Vector3 endPos = shakerZone.transform.position;

        Vector3 startScale = copyRT.localScale;
        Vector3 endScale = Vector3.zero;

        float duration = 0.4f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            copyRT.position = Vector3.Lerp(startPos, endPos, p);
            copyRT.localScale = Vector3.Lerp(startScale, endScale, p);

            yield return null;
        }

        copy.SetActive(false);
        copyRT.localScale = startScale; // вернуть размер для следующего раза
    }


    IEnumerator ReturnAndHide()
    {
        Vector3 startPos = copyRT.position;
        Vector3 endPos = spawnPoint;

        Vector3 startScale = copyRT.localScale;
        Vector3 endScale = new Vector3(0.7f, 0.7f, 1f);

        float duration = 0.4f;
        float t = 0f;

        CanvasGroup cg = copy.GetComponent<CanvasGroup>();
        if (cg == null) cg = copy.AddComponent<CanvasGroup>(); // вдруг забыли добавить

        cg.alpha = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            copyRT.position = Vector3.Lerp(startPos, endPos, p);
            copyRT.localScale = Vector3.Lerp(startScale, endScale, p);
            cg.alpha = 1f - p; // плавное исчезновение

            yield return null;
        }

        copy.SetActive(false);
        copyRT.localScale = startScale;
        cg.alpha = 1f; // сбрасываем на случай будущего использования
    }




    bool IsOverlapping(RectTransform a, RectTransform b)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(b, a.position, canvas.worldCamera);
    }
}

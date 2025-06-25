using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[System.Serializable]
public class CocktailRecipe
{
    public int id;
    public string name;
    public List<string> ingredients; // ровно 4 строки
}

[System.Serializable]
class CocktailData
{
    public List<CocktailRecipe> recipes = new();
}

public class ShakerZone : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image[] fillStates;           // 0-4 + overflow (6 шт.)
    [SerializeField] private RectTransform shakerImage;    // для анимации
    [SerializeField] private CocktailResultPanel resultPanel;

    [Header("JSON")]
    [SerializeField] private string jsonFileName = "cocktails.json";

    [Header("Мигание красным")]
    [SerializeField] private float blinkTime = 0.15f;
    [SerializeField] private int blinkCount = 2;

    private HashSet<string> current = new();
    private List<CocktailRecipe> recipes = new();

    private int lastID = -1;
    public string LastMixedName { get; private set; } = "";

    void Awake()
    {
        LoadRecipes();
        UpdateFillVisual();
    }

    public void TryAdd(string type)
    {
        if (current.Contains(type)) return;

        if (current.Count >= 4)
        {
            Debug.Log("Переполнение!");
            StartCoroutine(BlinkOverflow());
            return;
        }

        current.Add(type);
        UpdateFillVisual();
        Debug.Log($"Добавили: {type}");
    }

    public void Mix()
    {
        if (current.Count != 4)
        {
            Debug.Log("Нужно ровно 4 ингредиента.");
            StartCoroutine(BlinkOverflow());
            return;
        }

        foreach (var r in recipes)
        {
            if (IsSameSet(current, r.ingredients))
            {
                lastID = r.id;
                LastMixedName = r.name;
                Debug.Log($"Смешан коктейль: {LastMixedName} (ID: {lastID})");
                StartCoroutine(ShakeThenReset());
                return;
            }
        }

        lastID = -1;
        LastMixedName = "???";
        Debug.Log("Коктейль не определён.");
        StartCoroutine(ShakeThenReset());
    }

    void UpdateFillVisual()
    {
        foreach (var img in fillStates) img.gameObject.SetActive(false);
        int idx = Mathf.Clamp(current.Count, 0, 5);
        fillStates[idx].gameObject.SetActive(true);
    }

    IEnumerator BlinkOverflow()
    {
        Image red = fillStates[5];
        CanvasGroup cg = red.GetComponent<CanvasGroup>() ?? red.gameObject.AddComponent<CanvasGroup>();
        red.gameObject.SetActive(true);

        for (int i = 0; i < blinkCount; i++)
        {
            yield return Fade(cg, 0f, 1f);
            yield return Fade(cg, 1f, 0f);
        }

        red.gameObject.SetActive(false);
        cg.alpha = 1f;
        UpdateFillVisual();
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to)
    {
        float t = 0f;
        while (t < blinkTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / blinkTime);
            yield return null;
        }
    }

    IEnumerator ShakeThenReset()
    {
        Vector3 basePos = shakerImage.anchoredPosition;
        float t = 0f;
        float dur = 0.4f;
        float str = 12f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float off = Mathf.Sin(t * 40f) * str;
            shakerImage.anchoredPosition = basePos + Vector3.up * off;
            yield return null;
        }

        shakerImage.anchoredPosition = basePos;

        // показать результат
        resultPanel.Show(lastID, LastMixedName);

        current.Clear();
        UpdateFillVisual();
    }

    void LoadRecipes()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Файл не найден: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<CocktailData>(json);
        if (data != null && data.recipes != null)
            recipes = data.recipes;
        else
            Debug.LogWarning("Не удалось загрузить JSON.");
    }

    bool IsSameSet(HashSet<string> a, List<string> b)
        => a.Count == b.Count && b.All(a.Contains);
}

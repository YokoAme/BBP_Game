using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SortSceneManager : MonoBehaviour
{
    /* ---------- references ---------- */
    [Header("References")]
    public Button paperStackButton;
    public GameObject paperPrefab;
    public Transform paperParent;
    public SlideInUI paperStackSlide;
    public CanvasGroup gameplayGroup;
    public ScreenFade screenFade;

    /* ---------- settings ---------- */
    [Header("Settings")]
    public string jsonFilePath = "Day1_Cards.json"; // будет заменён в Awake
    public string nextSceneName = "LunchScene";
    public float gameFade = 0.4f;

    /* ---------- state ---------- */
    private readonly List<DossierCardData> cards = new();
    private readonly List<GameObject> spawnedCards = new();
    private int currentIndex = 0;
    private bool isSpawning = false;

    /* ========================================================= */
    /*                        A W A K E                           */
    /* ========================================================= */
    void Awake()
    {
        Debug.Log("<color=lime>[AWAKE] SortSceneManager вызвался!</color>");

        int day = SaveSystem.Instance.Data.currentDay;
        Debug.Log($"<color=cyan>SortScene ▶ day = {day}</color>");

        jsonFilePath = $"Day{day}_Cards.json";
        nextSceneName = $"LunchSceneD{day}";

        gameplayGroup.alpha = 0;
        gameplayGroup.interactable = false;
        gameplayGroup.blocksRaycasts = false;
    }

    /* ========================================================= */
    IEnumerator Start()
    {
        Debug.Log("<color=lime>Start()  — вошёл</color>");

        // 💡 Очистка очков за текущий день
        ScoreManager.Instance?.ResetDayScore();

        LoadCardsFromJson();
        Debug.Log("<color=lime>Start()  — Load закончился</color>");

        yield return new WaitForSeconds(screenFade.fadeDuration);
        yield return StartCoroutine(FadeCanvas(gameplayGroup, 0, 1, gameFade));

        if (!paperStackSlide.gameObject.activeSelf)
            paperStackSlide.gameObject.SetActive(true);

        paperStackSlide.Play();
        paperStackButton.onClick.AddListener(SpawnNextCard);
    }


    /* ========================================================= */
    /*                    JSON  →  cards                         */
    /* ========================================================= */
    void LoadCardsFromJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFilePath);
        Debug.Log($"<color=orange>Load {path}</color>");

        if (!File.Exists(path))
        {
            Debug.LogError("❌ JSON не найден!");
            return;
        }

        string json = File.ReadAllText(path);
        var rawCards = JsonHelper.FromJson<RawDossierCardData>(json);
        Debug.Log($"<color=orange>rawCards.Length = {rawCards.Length}</color>");

        cards.Clear();
        foreach (var raw in rawCards)
        {
            if (!System.Enum.TryParse(raw.correctFolder, true, out FolderType folderEnum))
            {
                Debug.LogWarning($"⚠️  Не удалось распарсить '{raw.correctFolder}', использую Red");
                folderEnum = FolderType.Red;
            }
            cards.Add(new DossierCardData { spriteName = raw.spriteName, correctFolder = folderEnum });
        }
    }

    /* ========================================================= */
    /*                     SPAWN  &  ANIMATE                     */
    /* ========================================================= */
    void SpawnNextCard()
    {
        if (isSpawning) return;
        isSpawning = true;

        if (currentIndex >= cards.Count)
        {
            paperStackButton.gameObject.SetActive(false);
            isSpawning = false;
            return;
        }

        var data = cards[currentIndex];
        GameObject paper = Instantiate(paperPrefab, paperParent);
        var rt = paper.GetComponent<RectTransform>();

        Vector2 stackPos = ((RectTransform)paperStackButton.transform).anchoredPosition;
        rt.anchoredPosition = new Vector2(stackPos.x, stackPos.y - 500f);
        rt.localScale = Vector3.one;
        paper.transform.SetAsLastSibling();

        var img = paper.GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>("Dossiers/" + data.spriteName);

        var meta = paper.GetComponent<CardMeta>() ?? paper.AddComponent<CardMeta>();
        meta.correctFolder = data.correctFolder;

        spawnedCards.Add(paper);
        currentIndex++;

        StartCoroutine(AnimateCard(rt));
        StartCoroutine(UnlockSpawn());

        if (currentIndex >= cards.Count)
            paperStackButton.gameObject.SetActive(false);
    }

    IEnumerator AnimateCard(RectTransform rt)
    {
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = Vector2.zero;
        Vector3 startScale = new Vector3(0.5f, 0.5f, 1f);
        Vector3 endScale = Vector3.one;

        float t = 0f, dur = 0.25f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = t / dur;
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, k);
            rt.localScale = Vector3.Lerp(startScale, endScale, k);
            yield return null;
        }

        rt.anchoredPosition = endPos;
        rt.localScale = endScale;
    }

    IEnumerator UnlockSpawn()
    {
        yield return new WaitForSeconds(0.1f);
        isSpawning = false;
    }

    /* ========================================================= */
    public void UnregisterCard(GameObject card) => spawnedCards.Remove(card);

    public void CheckForCompletion()
    {
        spawnedCards.RemoveAll(c => c == null || c.Equals(null));
        if (currentIndex >= cards.Count && spawnedCards.Count == 0)
            StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        // Запоминаем очки за день
        ScoreManager.Instance?.CommitScoreToSave();
        SaveSystem.Instance.Save();

        // 🎯 Показать общее количество очков
        Debug.Log($"<color=yellow>⭐ Всего очков: {SaveSystem.Instance.Data.score}</color>");

        screenFade.gameObject.SetActive(true);
        yield return screenFade.FadeIn();
        SceneManager.LoadScene(nextSceneName);
    }


    /* ========================================================= */
    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0f;
        cg.alpha = from;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    /* ---------- helper struct ---------- */
    [System.Serializable]
    public class RawDossierCardData
    { public string spriteName; public string correctFolder; }
}

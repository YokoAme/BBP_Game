using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BarScene2_Day4 : MonoBehaviour
{
    [Header("Ссылки")]
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;

#if UNITY_EDITOR
    /* ===== DEBUG: тестовые значения  ===== */
    [Header("DEBUG / Тестовые значения (только в редакторе)")]
    public bool debugOverride = false;

    [SerializeField] private int  debugScore  = 12;
    [SerializeField] private bool debugHasKey = true;

    [Space(6)]
    [Tooltip("Принудительно установить текущий день = 4")]
    [SerializeField] private bool forceDay4 = false;
#endif

    /* ---------------------------------------------------- */

    void Awake()
    {
        /* 1. всегда гарантируем, что SaveSystem существует */
        if (SaveSystem.Instance == null)
            new GameObject("SaveSystem").AddComponent<SaveSystem>();

        /* 2. подхватываем ссылки, если забыли проставить в инспекторе */
        if (screenFade == null) screenFade = FindObjectOfType<ScreenFade>();
        if (dialogueManager == null) dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
#if UNITY_EDITOR
        /* ---------- DEBUG: форсим 4-й день ---------- */
        if (forceDay4)
        {
            SaveSystem.Instance.Data.currentDay = 4;
            SaveSystem.Instance.Save();
            Debug.Log("<color=cyan>[DEBUG] currentDay = 4 (forced)</color>");
        }
#endif

        /* ---------- проверяем ссылки ---------- */
        if (screenFade == null || dialogueManager == null)
        {
            Debug.LogError("ScreenFade или DialogueManager не найдены!");
            yield break;
        }

        /* ---------- основная логика сцены ---------- */
        yield return screenFade.FadeOut();

        dialogueManager.StartDialogue("Dialogue_BarDay4_2.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        /* напиток / ключ */
        int cookedId = GameFlow.CookedDrinkID;
        bool keyGiven = cookedId == 3;
        string line = keyGiven ?
              "Да: Твоя взяла. Держи ключи."
            : "Нет: Ну видно не судьба, сам я лезть туда не буду.";

        if (keyGiven)
        {
            SaveSystem.Instance.Data.hasBossKey = true;
            SaveSystem.Instance.Save();
        }

        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("rich", line);
        yield return WaitForClick();

        dialogueManager.StartDialogue("Dialogue_BarDay4_2.json");
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        yield return screenFade.FadeIn();

        /* ---------- читаем реальные (или debug) данные ---------- */
        bool hasKey = SaveSystem.Instance.Data.hasBossKey;
        int score = SaveSystem.Instance.Data.score;

#if UNITY_EDITOR
        if (debugOverride)
        {
            hasKey = debugHasKey;
            score  = debugScore;
            Debug.Log($"<color=cyan>[DEBUG override] key={hasKey}, score={score}</color>");
        }
#endif

        /* ---------- выбираем концовку ---------- */
        string ending =
            !hasKey ? "Ending_Goriy" :   // нет ключа
            score > 10 ? "Ending_Yoka" :   // ключ есть + много очков
                           "Ending_Boss";    // ключ есть + мало очков


        // Сохраняем как последнюю сцену
        SaveSystem.Instance.Data.lastScene = ending;
        SaveSystem.Instance.Save();

        Debug.Log($"<color=yellow>→ Концовка: {ending} (key={hasKey}, score={score})</color>");
        SceneManager.LoadScene(ending);

    }

    IEnumerator WaitForClick() =>
        new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
}

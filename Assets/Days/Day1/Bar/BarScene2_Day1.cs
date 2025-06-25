using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BarScene2_Day1 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject shepsSprite;
    public GameObject yukiSprite;

    private Dictionary<int, string> drinkNamesById;
    private const int shepsEndLine = 3;      // после этой строки Шепс уходит (0-based)

    void Start()
    {
        LoadCocktailNames();
        StartCoroutine(RunScene());
    }

    IEnumerator RunScene()
    {
        /* ---------- фейд-ин ---------- */
        yield return screenFade.FadeOut();

        /* ---------- Шепс уже в кадре ---------- */
        shepsSprite.SetActive(true);
        yukiSprite.SetActive(false);

        /* ---------- загружаем JSON (только для цветов) ---------- */
        dialogueManager.StartDialogue("Dialogue_BarDay1_2.json");
        yield return null;                       // один кадр — characterDict готов
        dialogueManager.ShowPanel(false);        // скрываем, чтобы не вспыхнула 1-я строка

        /* ---------- кастомная фраза Шепса ---------- */
        int cooked = GameFlow.CookedDrinkID;
        int requested = GameFlow.RequestedDrinkID;

        string firstLine = (requested == -1 || cooked == requested)
            ? $"О так это же тот самый – «{GetDrinkName(cooked)}»."
            : "Если честно, не могу понять вкус... ты придумала новый коктейль?";

        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("mistersheps", firstLine);
        yield return WaitForClick();

        /* ---------- выводим оставшиеся 3 строки Шепса ---------- */
        dialogueManager.StartDialogue("Dialogue_BarDay1_2.json");

        /* ждём, пока менеджер дойдёт ровно до 3-й строки (index == 3)         */
        /* игрок ещё видит её, но панель уже НЕ реагирует на клики              */
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == shepsEndLine);
        dialogueManager.dialoguePanel.interactable = false;   // блокируем ввод

        /* ---------- пауза 0.6 c, затем прячем панель и уводим Шепса ---------- */
        yield return new WaitForSeconds(0.6f);
        dialogueManager.ShowPanel(false);

        if (shepsSprite.TryGetComponent<SpriteEnter>(out var seSheps))
        {
            seSheps.ExitToRight();
            yield return new WaitForSeconds(seSheps.duration + 0.1f);
        }
        shepsSprite.SetActive(false);

        /* ---------- Юки входит справа ---------- */
        yukiSprite.SetActive(true);
        float waitEntrance = 0.6f;
        if (yukiSprite.TryGetComponent<SpriteEnter>(out var seYuki))
        {
            seYuki.EnterFromRight();
            waitEntrance = seYuki.duration + 0.1f;
        }
        yield return new WaitForSeconds(waitEntrance);

        /* ---------- снова показываем панель  и вручную переключаем строку ---------- */
        dialogueManager.ShowPanel(true);
        dialogueManager.dialoguePanel.interactable = true;    // возвращаем управление
        dialogueManager.ForceNextLine();
        // теперь первая реплика Юки

        /* ---------- ждём конца всего диалога ---------- */
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        /* ---------- затемнение → CookingScene ---------- */
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene2";
        UnityEngine.SceneManagement.SceneManager.LoadScene("CookingScene");

    }

    /* ---------- утилиты ---------- */
    IEnumerator WaitForClick()
    {
        yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
    }

    void LoadCocktailNames()
    {
        drinkNamesById = new Dictionary<int, string>();
        string path = Path.Combine(Application.streamingAssetsPath, "cocktails.json");
        if (File.Exists(path))
        {
            var db = JsonUtility.FromJson<CocktailDatabase>(File.ReadAllText(path));
            foreach (var r in db.recipes) drinkNamesById[r.id] = r.name;
        }
        else
            Debug.LogError("cocktails.json not found: " + path);
    }

    string GetDrinkName(int id) =>
        drinkNamesById.TryGetValue(id, out var n) ? n : "Неизвестный коктейль";

    /* ---------- JSON-структуры ---------- */
    [System.Serializable] public class CocktailRecipe { public int id; public string name; public List<string> ingredients; }
    [System.Serializable] public class CocktailDatabase { public List<CocktailRecipe> recipes; }
}
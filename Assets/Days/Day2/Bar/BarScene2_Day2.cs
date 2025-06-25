using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BarScene2_Day2 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject goriySprite;
    public GameObject yukiSprite;

    private Dictionary<int, string> drinkNamesById;

    void Start()
    {
        LoadCocktailNames();
        StartCoroutine(RunScene());
    }

    IEnumerator RunScene()
    {
        // 1. затемнение
        yield return screenFade.FadeOut();

        // 2. включаем Гория
        goriySprite.SetActive(true);
        yukiSprite.SetActive(false);

        // 3. загружаем characterDict
        dialogueManager.StartDialogue("Dialogue_BarDay2_2.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        // 4. кастомная фраза Гория
        int cooked = GameFlow.CookedDrinkID;
        string firstLine = cooked == 1
            ? "Вот это вкус — напоминает мне о том, как мы проводили время вместе. Как же было тогда хорошо."
            : "Видимо, ты уже не понимаешь меня с полуслова. Ну ладно, всё равно расскажу.";

        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("goriy", firstLine);
        yield return WaitForClick();

        // 5. продолжаем обычный диалог
        dialogueManager.StartDialogue("Dialogue_BarDay2_2.json");

        // 6. ждём до появления Юки (27-я строка)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 27);

        // 7. Юки появляется
        yukiSprite.SetActive(true);
        if (yukiSprite.TryGetComponent<SpriteEnter>(out var seYuki))
        {
            seYuki.EnterFromRight();
            yield return new WaitForSeconds(seYuki.duration + 0.1f);
        }

        // 8. ждём завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 9. затемнение и переход
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene2D2";
        UnityEngine.SceneManagement.SceneManager.LoadScene("CookingScene");
    }

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
            foreach (var r in db.recipes)
                drinkNamesById[r.id] = r.name;
        }
        else
            Debug.LogError("cocktails.json not found: " + path);
    }

    string GetDrinkName(int id) =>
        drinkNamesById.TryGetValue(id, out var n) ? n : "Неизвестный коктейль";

    [System.Serializable] public class CocktailRecipe { public int id; public string name; public List<string> ingredients; }
    [System.Serializable] public class CocktailDatabase { public List<CocktailRecipe> recipes; }
}

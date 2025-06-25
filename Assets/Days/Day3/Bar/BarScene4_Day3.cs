using UnityEngine;
using System.Collections;

public class BarScene4_Day3 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject mishaSprite;

    private readonly int[] grapeDrinkIDs = { 1, 5, 6, 9, 10, 12, 13 };

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();

        mishaSprite.SetActive(true);

        // Загружаем characterDict
        dialogueManager.StartDialogue("Dialogue_BarDay3_4.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        // Проверяем, виноградный ли напиток
        int cooked = GameFlow.CookedDrinkID;

        string mishaReaction = System.Array.Exists(grapeDrinkIDs, id => id == cooked)
            ? "Беее, зачем я заказал это, я же не люблю виноград."
            : "А в этом мире вкус отличается от земного, мне нравится виноград!";

        // Первая реакция Миши
        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("misha", mishaReaction);
        yield return WaitForClick();

        // Запускаем основной диалог
        dialogueManager.StartDialogue("Dialogue_BarDay3_4.json");

        // Ждём до реплики «Ну ладно, я побежал» (index 7)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 8);

        // Уводим Мишу
        dialogueManager.dialoguePanel.interactable = false;
        yield return new WaitForSeconds(0.6f);
        dialogueManager.ShowPanel(false);

        if (mishaSprite.TryGetComponent<SpriteEnter>(out var mishaEnter))
        {
            mishaEnter.ExitToRight();
            yield return new WaitForSeconds(mishaEnter.duration + 0.1f);
        }
        mishaSprite.SetActive(false);

        // Пауза → возвращаем панель → две финальные реплики Ёки
        yield return new WaitForSeconds(1.0f);
        dialogueManager.ShowPanel(true);
        dialogueManager.dialoguePanel.interactable = true;

        yield return WaitForClick(); // первая реплика Ёки
        yield return WaitForClick(); // вторая реплика Ёки

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeSceneD3");
    }

    IEnumerator WaitForClick() =>
        new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
}

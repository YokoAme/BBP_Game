using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BarScene3_Day2 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject yukiSprite;
    public GameObject goriySprite;

    private const int favDrinkID = 4;  // Яблоко

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();  // затемнение

        yukiSprite.SetActive(true);
        goriySprite.SetActive(true);

        // загружаем characterDict
        dialogueManager.StartDialogue("Dialogue_BarDay2_3.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        // Кастомная реакция Юки
        string yukiLine = (GameFlow.CookedDrinkID == favDrinkID)
            ? "Ура–ура, наконец–то я чувствую вкус."
            : "Как–то не яблочно в названии этого напитка.";


        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("yuki", yukiLine);
        yield return WaitForClick();

        // Горий — вторая и третья реплики из JSON
        dialogueManager.StartDialogue("Dialogue_BarDay2_3.json");
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 3);  // index 0 и 1 — две реплики Гория

        // Уход Гория вправо
        if (goriySprite.TryGetComponent<SpriteEnter>(out var goriyEnter))
        {
            goriyEnter.ExitToRight();
            yield return new WaitForSeconds(goriyEnter.duration + 0.1f);
        }
        goriySprite.SetActive(false);

        // Ждём, когда Юки скажет, что уходит (индекс 25)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 27);

        // Уход Юки вправо
        if (yukiSprite.TryGetComponent<SpriteEnter>(out var yukiEnter))
        {
            yukiEnter.ExitToRight();
            yield return new WaitForSeconds(yukiEnter.duration + 0.1f);
        }
        yukiSprite.SetActive(false);

        // Ждём завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // Затемнение и переход
        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeSceneD2"); // Укажи нужную сцену здесь
    }

    IEnumerator WaitForClick() =>
        new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
}

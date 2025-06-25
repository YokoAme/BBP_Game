using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BarScene4_Day1 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject goriySprite;
    public GameObject yukiSprite;


    private const int specialDrinkID = 14;  // допустим "Счастье" — ID = 3

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();

        goriySprite.SetActive(true);
        yukiSprite.SetActive(true);


        // ---------- загружаем characterDict ---------- 
        dialogueManager.StartDialogue("Dialogue_BarDay1_4.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        // ---------- кастомная реакция Гория на коктейль ----------
        string reaction = GameFlow.CookedDrinkID == specialDrinkID
            ? "О, наконец–то, тот самый вкус моей юности. "
            : "О как, не думал что в каждом баре рецепт этого напитка в корне отличается от оригинала.";

        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("goriy", reaction);
        yield return WaitForClick();

        dialogueManager.StartDialogue("Dialogue_BarDay1_4.json");

        // ждём пока дойдёт до предпоследней строки (всего 9 строк → индекс 7)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 7);

        // Уходят Юки и Горий
        if (yukiSprite.TryGetComponent<SpriteEnter>(out var yukiEnter))
        {
            yukiEnter.ExitToLeft();
            yield return new WaitForSeconds(yukiEnter.duration + 0.1f);
        }
        yukiSprite.SetActive(false);

        if (goriySprite.TryGetComponent<SpriteEnter>(out var goriyEnter))
        {
            goriyEnter.ExitToLeft();
            yield return new WaitForSeconds(goriyEnter.duration + 0.1f);
        }
        goriySprite.SetActive(false);

        // ждём финальные 2 строки (пустую + Ёка)
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);


        // ---------- затемнение и переход ----------
        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }

    IEnumerator WaitForClick() =>
        new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
}

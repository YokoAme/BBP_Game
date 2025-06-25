using UnityEngine;
using System.Collections;

public class BarScene2_Day3 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;

    void Start()
    {
        StartCoroutine(RunScene());
    }

    IEnumerator RunScene()
    {
        // 1. затемнение
        yield return screenFade.FadeOut();

        // 2. Запустить диалог — пока скрыт
        dialogueManager.StartDialogue("Dialogue_BarDay3_2.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        // 3. Вставка реплики Нины в зависимости от напитка
        int cookedId = GameFlow.CookedDrinkID;
        string ninaLine = cookedId == 13
            ? "Ну хоть где–то я могу насладиться этим вкусом. В последнее время меня вообще ничего не торкает."
            : "Ой, это совсем не Заморозка, обманывать меня не надо!";

        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("nina", ninaLine);
        yield return WaitForClick();

        // 4. Продолжение диалога как обычно
        dialogueManager.StartDialogue("Dialogue_BarDay3_2.json");

        // 5. Ждём завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 6. затемнение и переход
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene2D3";
        UnityEngine.SceneManagement.SceneManager.LoadScene("CookingScene");
    }

    IEnumerator WaitForClick()
    {
        yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
    }
}

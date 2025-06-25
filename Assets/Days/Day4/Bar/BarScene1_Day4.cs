using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BarScene_Day4 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject backgroundBar;

    public GameObject eliadarSprite; // Спрайт загадочного ??? / Элиадар
    public GameObject richSprite;    // Спрайт Рича

    void Start()
    {
        StartCoroutine(StartBarScene());
    }

    IEnumerator StartBarScene()
    {
        // 1. Затемнение
        yield return screenFade.FadeOut();

        // 2. Фон бара
        backgroundBar.SetActive(true);

        // 3. Элиадар входит первым (справа)
        eliadarSprite.SetActive(true);
        if (eliadarSprite.TryGetComponent<SpriteEnter>(out var eliEnter))
        {
            eliEnter.EnterFromRight();
            yield return new WaitForSeconds(eliEnter.duration + 0.1f);
        }

        // 4. Старт диалога
        dialogueManager.StartDialogue("Dialogue_BarDay4_1.json");

        // 5. Ждём до момента, когда Элиадар говорит "До встречи" (например, index 13)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 24);
        dialogueManager.dialoguePanel.interactable = false;
        yield return new WaitForSeconds(0.5f);
        dialogueManager.ShowPanel(false);

        // 6. Элиадар уходит вправо
        if (eliadarSprite.TryGetComponent<SpriteEnter>(out var eliExit))
        {
            eliExit.ExitToRight();
            yield return new WaitForSeconds(eliExit.duration + 0.1f);
        }
        eliadarSprite.SetActive(false);

        // 7. Пауза и возвращение панели
        yield return new WaitForSeconds(0.6f);
        dialogueManager.ShowPanel(true);
        dialogueManager.dialoguePanel.interactable = true;

        // 8. Продолжение реплик Ёки (например, index 14 и 15)
        yield return WaitForClick();
        yield return WaitForClick();

        // 9. Рич входит справа
        richSprite.SetActive(true);
        if (richSprite.TryGetComponent<SpriteEnter>(out var richEnter))
        {
            richEnter.EnterFromRight();
            yield return new WaitForSeconds(richEnter.duration + 0.1f);
        }

        // 10. Ждём завершения всего диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);



        // 12. Фейд и переход в следующую сцену
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene1D4";
        SceneManager.LoadScene("CookingScene");
    }

    IEnumerator WaitForClick()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
    }
}

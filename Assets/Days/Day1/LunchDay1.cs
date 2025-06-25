using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LunchDay1 : MonoBehaviour
{
    public ScreenFade screenFade;                 // Скрипт фейда
    public DialogueManager dialogueManager;       // Диалоговый менеджер
    public GameObject backgroundImage;            // Фон офиса
    public GameObject nikaSprite;                 // Босс
    public GameObject yokaSprite;                 // Ёка

    void Start()
    {

        StartCoroutine(RunDaySequence());
    }

    IEnumerator RunDaySequence()
    {
        // 1. Затемнение (из чёрного)
        yield return screenFade.FadeOut();

        // 2. Показать фон (если он был скрыт)
        backgroundImage.SetActive(true);

        // 3. Появление босса справа
        nikaSprite.SetActive(true);
        nikaSprite.GetComponent<SpriteEnter>().EnterFromRight();
        yield return new WaitForSeconds(0.6f);

        // 4. Запустить первую реплику
        // Показать панель с диалогом плавно
        dialogueManager.GetComponent<CanvasGroup>().alpha = 1;
        dialogueManager.GetComponent<CanvasGroup>().interactable = true;
        dialogueManager.GetComponent<CanvasGroup>().blocksRaycasts = true;

        dialogueManager.StartDialogue("Dialogue_LunchDay1.json");

        // 5. Подождать первую реплику (вручную или дождаться нажатия)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 2);

        // 6. Появление Ёки слева
        yokaSprite.SetActive(true);
        yokaSprite.GetComponent<SpriteEnter>().EnterFromLeft();
        yield return new WaitForSeconds(0.6f);

        // 7. Ждать завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 8. Фейд в чёрное
        yield return screenFade.FadeIn();

        // 9. Переход в следующую сцену
        SceneManager.LoadScene("BarScene");
    }
}

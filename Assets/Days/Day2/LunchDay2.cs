using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LunchDay2 : MonoBehaviour
{
    public ScreenFade screenFade;                 // Скрипт фейда
    public DialogueManager dialogueManager;       // Диалоговый менеджер
    public GameObject backgroundImage;            // Фон офиса
    public GameObject nikaSprite;                 // Ника
    public GameObject yokaSprite;                 // Ёка

    void Start()
    {
        StartCoroutine(RunDaySequence());
    }

    IEnumerator RunDaySequence()
    {
        // 1. Затемнение (из чёрного)
        yield return screenFade.FadeOut();

        // 2. Показать фон
        backgroundImage.SetActive(true);

        // 3. Появление Ёки слева
        yokaSprite.SetActive(true);
        yokaSprite.GetComponent<SpriteEnter>().EnterFromLeft();
        yield return new WaitForSeconds(0.6f);

        // 4. Появление Ники справа
        nikaSprite.SetActive(true);
        nikaSprite.GetComponent<SpriteEnter>().EnterFromRight();
        yield return new WaitForSeconds(0.6f);

        // 5. Показать диалоговую панель
        dialogueManager.GetComponent<CanvasGroup>().alpha = 1;
        dialogueManager.GetComponent<CanvasGroup>().interactable = true;
        dialogueManager.GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 6. Запуск диалога
        dialogueManager.StartDialogue("Dialogue_LunchDay2.json");

        // 7. Ждать завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 8. Фейд в чёрное
        yield return screenFade.FadeIn();

        // 9. Переход в следующую сцену
        SceneManager.LoadScene("BarSceneD2");
    }
}

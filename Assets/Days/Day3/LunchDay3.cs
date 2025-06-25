using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LunchDay3 : MonoBehaviour
{
    public ScreenFade screenFade;               // Скрипт фейда
    public DialogueManager dialogueManager;     // Диалоговый менеджер
    public GameObject backgroundImage;          // Фон обеда
    public GameObject richSprite;               // Рич
    public GameObject yokaSprite;               // Ёка

    void Start()
    {
        StartCoroutine(RunDaySequence());
    }

    IEnumerator RunDaySequence()
    {
        // 1. Затемнение
        yield return screenFade.FadeOut();

        // 2. Включаем фон
        backgroundImage.SetActive(true);

        // 3. Появление Ёки слева
        yokaSprite.SetActive(true);
        yokaSprite.GetComponent<SpriteEnter>().EnterFromLeft();
        yield return new WaitForSeconds(0.6f);

        // 4. Подготовка панели диалога
        var canvasGroup = dialogueManager.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 5. Запуск диалога
        dialogueManager.StartDialogue("Dialogue_LunchDay3.json");

        // 6. Ждать появления реплики Рича (4-я строка, индекс 3)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 3);

        // 7. Появление Рича справа
        richSprite.SetActive(true);
        richSprite.GetComponent<SpriteEnter>().EnterFromRight();
        yield return new WaitForSeconds(0.6f);

        // 8. Дождаться конца диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 9. Фейд и переход
        yield return screenFade.FadeIn();
        SceneManager.LoadScene("BarSceneD3");
    }
}

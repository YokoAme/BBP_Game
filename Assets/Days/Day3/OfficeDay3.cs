using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfficeDay3 : MonoBehaviour
{
    public ScreenFade screenFade;                 // Скрипт фейда
    public DialogueManager dialogueManager;       // Диалоговый менеджер
    public GameObject backgroundImage;            // Фон офиса
    public GameObject bossSprite;                 // Босс
    public GameObject yokaSprite;                 // Ёка

    void Start()
    {



        // Принудительно установить текущий день, если SaveSystem ещё не создан
        if (SaveSystem.Instance == null)
        {
            GameObject go = new GameObject("SaveSystem");
            go.AddComponent<SaveSystem>();
        }

        // Пример: установить день 2, если ты запускаешь сцену вручную
        if (SaveSystem.Instance.Data.currentDay <= 0)
        {
            SaveSystem.Instance.Data.currentDay = 3;  // ← поставь нужный день
            Debug.Log("<color=orange>Установлен день вручную для теста: 3</color>");
        }


        StartCoroutine(RunDaySequence());
    }

    IEnumerator RunDaySequence()
    {
        // 1. Затемнение (из чёрного)
        yield return screenFade.FadeOut();

        // 2. Показать фон (если он был скрыт)
        backgroundImage.SetActive(true);

        // 3. Появление босса слева
        bossSprite.SetActive(true);
        bossSprite.GetComponent<SpriteEnter>().EnterFromRight();

        yield return new WaitForSeconds(0.6f);

        // 4. Показать панель с диалогом
        dialogueManager.GetComponent<CanvasGroup>().alpha = 1;
        dialogueManager.GetComponent<CanvasGroup>().interactable = true;
        dialogueManager.GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 5. Запустить первую реплику
        dialogueManager.StartDialogue("Dialogue_OfficeDay3.json");

        // 7. Появление Ёки справа
        yokaSprite.SetActive(true);
        yokaSprite.GetComponent<SpriteEnter>().EnterFromLeft();
        yield return new WaitForSeconds(0.6f);
        // 6. Подождать первую реплику (пока игрок нажмёт)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 2);


        // 8. Ждать завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 9. Фейд в чёрное
        yield return screenFade.FadeIn();

        // 10. Переход в следующую сцену
        SceneManager.LoadScene("SortScene");
    }

}

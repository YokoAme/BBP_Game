using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfficeDay4 : MonoBehaviour
{
    public ScreenFade screenFade;                 // Скрипт фейда
    public DialogueManager dialogueManager;       // Диалоговый менеджер
    public GameObject backgroundImage;            // Фон офиса
    public GameObject yokaSprite;                 // Ёка

    void Start()
    {
        // Принудительно установить текущий день, если SaveSystem ещё не создан
        if (SaveSystem.Instance == null)
        {
            GameObject go = new GameObject("SaveSystem");
            go.AddComponent<SaveSystem>();
        }

      
         SaveSystem.Instance.Data.currentDay = 4;
         Debug.Log("<color=orange>Установлен день вручную для теста: 4</color>");
        

        StartCoroutine(RunDaySequence());
    }

    IEnumerator RunDaySequence()
    {
        // 1. Затемнение (из чёрного)
        yield return screenFade.FadeOut();

        // 2. Показать фон
        backgroundImage.SetActive(true);

        // 3. Появление Ёки справа
        yokaSprite.SetActive(true);
        yokaSprite.GetComponent<SpriteEnter>().EnterFromLeft();
        yield return new WaitForSeconds(0.6f);

        // 4. Показать диалоговую панель
        var panel = dialogueManager.GetComponent<CanvasGroup>();
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;

        // 5. Запустить диалог
        dialogueManager.StartDialogue("Dialogue_OfficeDay4.json");

        // 6. Ждём хотя бы 2 строки (например, "Странно, никого нет." и "Обычно босс встречал меня.")
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 2);

        // 7. Ждём окончания диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 8. Затемнение
        yield return screenFade.FadeIn();

        // 9. Переход на следующую сцену
        SceneManager.LoadScene("SortScene");
    }
}

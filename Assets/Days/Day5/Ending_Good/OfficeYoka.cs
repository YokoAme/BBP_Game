using UnityEngine;
using System.Collections;

public class OfficeYoka : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;

    public CanvasGroup backgroundCorridor;
    public CanvasGroup backgroundBossDoor;
    public CanvasGroup backgroundOffice;
    public CanvasGroup backgroundSoulClose;
    public CanvasGroup backgroundRescue;
    public CanvasGroup backgroundFinal;


    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();

        // Старт — коридор (без фейда)
        backgroundCorridor.gameObject.SetActive(true);
        backgroundCorridor.alpha = 1f;

        dialogueManager.StartDialogue("Dialogue_OfficeYoka.json");

        // Дверь босса
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 5);
        backgroundBossDoor.gameObject.SetActive(true);
        yield return FadeIn(backgroundBossDoor);

        // Кабинет
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 9);
        backgroundOffice.gameObject.SetActive(true);
        yield return FadeIn(backgroundOffice);

        // Душа крупно
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 15);
        backgroundSoulClose.gameObject.SetActive(true);
        yield return FadeIn(backgroundSoulClose);

        // Спасение
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 23);
        backgroundRescue.gameObject.SetActive(true);
        yield return FadeIn(backgroundRescue);

        // Спасение
        // после реплик, но перед финальным фейдом и загрузкой главного меню:
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 44); // индекс последней текстовой реплики
        backgroundFinal.gameObject.SetActive(true);
        yield return FadeIn(backgroundFinal);
        

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // Сохраняем прогресс
        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeYoka";
        sd.currentDay = 5;
        SaveSystem.Instance.Save();

        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    IEnumerator FadeIn(CanvasGroup canvas, float duration = 1f)
    {
        canvas.alpha = 0f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        canvas.alpha = 1f;
    }
}

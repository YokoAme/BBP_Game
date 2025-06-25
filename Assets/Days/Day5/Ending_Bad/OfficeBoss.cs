using UnityEngine;
using System.Collections;

public class OfficeBoss : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;

public CanvasGroup backgroundCorridor;
public CanvasGroup backgroundDevice;
public CanvasGroup backgroundSoulInfo;
    public CanvasGroup backgroundCityFinal;



    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();

        // Старт — коридор (без фейда)
        backgroundCorridor.gameObject.SetActive(true);
        backgroundCorridor.alpha = 1f;

        dialogueManager.StartDialogue("Dialogue_OfficeBoss.json");

        // После фразы "Пойдём – покажу."
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 4);
        backgroundDevice.gameObject.SetActive(true);
        yield return FadeIn(backgroundDevice);

        // После "Это пробойная душа..."
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 20);
        backgroundSoulInfo.gameObject.SetActive(true);
        yield return FadeIn(backgroundSoulInfo);


        // После "Покажи мне моё новое место работы."
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 42);
        backgroundCityFinal.gameObject.SetActive(true);
        yield return FadeIn(backgroundCityFinal);


        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // Сохраняем прогресс
        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeBoss";
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

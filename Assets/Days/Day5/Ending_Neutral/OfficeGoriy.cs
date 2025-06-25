using UnityEngine;
using System.Collections;

public class OfficeGoriy : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;

    public CanvasGroup backgroundCorridor;
    public CanvasGroup backgroundBossDoor;
    public CanvasGroup backgroundOffice;
    public CanvasGroup backgroundSoulClose;
    public CanvasGroup backgroundExplosion;
    public CanvasGroup backgroundHoldSoul;
    public CanvasGroup backgroundFarewellBoss;
    public CanvasGroup backgroundEpilogueYoka;

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();

        backgroundCorridor.gameObject.SetActive(true);
        backgroundCorridor.alpha = 1f;

        dialogueManager.StartDialogue("Dialogue_OfficeGoriy.json");

        // Кабинет
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 3);
        backgroundOffice.gameObject.SetActive(true);
        yield return FadeIn(backgroundOffice);

        // Душа крупно (после "Отпусти девочку")
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 18);
        backgroundSoulClose.gameObject.SetActive(true);
        yield return FadeIn(backgroundSoulClose);

        // Взрыв капсулы
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 33);
        backgroundExplosion.gameObject.SetActive(true);
        yield return FadeIn(backgroundExplosion);

        // Горий держит девочку
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 58);
        backgroundHoldSoul.gameObject.SetActive(true);
        yield return FadeIn(backgroundHoldSoul);

        // Прощание с Валентином
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 66);
        backgroundFarewellBoss.gameObject.SetActive(true);
        yield return FadeIn(backgroundFarewellBoss);

        // Финальный фон — дом Ёки
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 80);
        backgroundEpilogueYoka.gameObject.SetActive(true);
        yield return FadeIn(backgroundEpilogueYoka);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeGoriy";
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

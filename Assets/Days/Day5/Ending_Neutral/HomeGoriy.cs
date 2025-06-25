using UnityEngine;
using System.Collections;

public class HomeGoriy : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;

    public GameObject background1;                 // начальный фон (обычный)
    public CanvasGroup background2Canvas;          // фон про Виктора
    public CanvasGroup background3Canvas;          // финальный фон (засыпание)

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        // ---------- затемнение → появление сцены ----------
        yield return screenFade.FadeOut();

        background1.SetActive(true);               // включаем стартовый фон
        background2Canvas.alpha = 0f;
        background2Canvas.gameObject.SetActive(true); // второй фон прозрачный
        background3Canvas.alpha = 0f;
        background3Canvas.gameObject.SetActive(true); // третий фон прозрачный

        // ---------- запуск диалога ----------
        dialogueManager.StartDialogue("Dialogue_HomeGoriy.json");

        // ---------- смена на фон 2 — рассказ про Виктора ----------
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 16); // настрой на нужную строку
        yield return FadeToCanvas(background2Canvas);

        // ---------- смена на фон 3 — под финал, например когда говорит "Дядя ушёл..." ----------
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 23); // настрой по контексту
        yield return FadeToCanvas(background3Canvas);

        // ---------- ждём окончания диалога ----------
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // ---------- затемнение и переход ----------
        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeGoriy";
        sd.currentDay = 5;
        SaveSystem.Instance.Save();

        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DayTransition");
    }

    IEnumerator FadeToCanvas(CanvasGroup canvas, float duration = 1f)
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

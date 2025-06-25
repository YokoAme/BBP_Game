using UnityEngine;
using System.Collections;

public class HomeDoss : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject background1;
    public CanvasGroup background2Canvas;

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        // ---------- затемнение → появление сцены ----------
        yield return screenFade.FadeOut();

        background1.SetActive(true);
        background2Canvas.alpha = 0f;
        background2Canvas.gameObject.SetActive(true); // активируем второй фон, но он прозрачный

        // ---------- запуск диалога ----------
        dialogueManager.StartDialogue("Dialogue_HomeBoss.json");

        // ---------- ждём 6-й строки ----------
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 2);

        // ---------- плавная смена фона без фейда ----------
        float duration = 1f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            background2Canvas.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        background2Canvas.alpha = 1f;

        // ---------- ждём окончания диалога ----------
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // ---------- затемнение и переход к следующей сцене ----------

        /* ---------- сохраняем прогресс ---------- */
        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeBoss";   // куда пойдёт DayTransition
        sd.currentDay = 5;                 // следующий день
        SaveSystem.Instance.Save();        // пишем save.json


yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DayTransition"); // замени на нужную
    }
}

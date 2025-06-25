using UnityEngine;
using System.Collections;

public class IncScene_Day2 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject backgroundNika;              // начальный фон с Никой
    public CanvasGroup backgroundBossCanvas;       // фон, когда подходит Босс

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        // ---------- затемнение → появление сцены ----------
        yield return screenFade.FadeOut();

        backgroundNika.SetActive(true);
        backgroundBossCanvas.alpha = 0f;
        backgroundBossCanvas.gameObject.SetActive(true); // активируем второй фон, но он пока невидим

        // ---------- запуск диалога ----------
        dialogueManager.StartDialogue("Dialogue_Inc.json");

        // ---------- ждём 12-й строки — фраза «Это правда» ----------
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 12);

        // ---------- плавная смена фона (появление Босса) ----------
        float duration = 1.2f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            backgroundBossCanvas.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        backgroundBossCanvas.alpha = 1f;

        // ---------- ждём окончания диалога ----------
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // ---------- затемнение и переход к следующей сцене ----------

        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeSceneD3";   // куда пойдёт DayTransition
        sd.currentDay = 3;                 // следующий день
        SaveSystem.Instance.Save();        // пишем save.json

        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DayTransition"); // замени на нужную сцену
    }
}

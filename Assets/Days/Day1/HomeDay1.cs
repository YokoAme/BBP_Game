using UnityEngine;
using System.Collections;

public class RoomScene_Day1 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject background1;
    public CanvasGroup background2Canvas;

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        /* ---------- фейд-ин (из чёрного) ---------- */
        yield return screenFade.FadeOut();

        background1.SetActive(true);
        background2Canvas.alpha = 0f;
        background2Canvas.gameObject.SetActive(true);

        /* ---------- запускаем диалог ---------- */
        dialogueManager.StartDialogue("Dialogue_HomeDay1.json");

        /* ---------- ждём 9-й строки ---------- */
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 9);

        /* ---------- мягкая смена фона ---------- */
        float dur = 1f;
        for (float t = 0f; t < dur; t += Time.deltaTime)
        {
            background2Canvas.alpha = Mathf.Lerp(0f, 1f, t / dur);
            yield return null;
        }
        background2Canvas.alpha = 1f;

        /* ---------- ждём конца диалога ---------- */
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        /* ---------- сохраняем прогресс ---------- */
        var sd = SaveSystem.Instance.Data;
        sd.lastScene = "OfficeSceneD2";   // куда пойдёт DayTransition
        sd.currentDay = 2;                 // следующий день
        SaveSystem.Instance.Save();        // пишем save.json

        /* ---------- фейд-аут → DayTransition ---------- */
        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DayTransition");
    }
}

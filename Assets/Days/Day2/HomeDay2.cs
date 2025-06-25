using UnityEngine;
using System.Collections;

public class RoomScene_Day2 : MonoBehaviour
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
        dialogueManager.StartDialogue("Dialogue_HomeDay2.json");

        // ---------- ждём 6-й строки ----------
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex == 15);

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
        yield return screenFade.FadeIn();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Inc"); // замени на нужную
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BarScene3_Day1 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject yukiSprite;
    public GameObject goriySprite;

    private const int favDrinkID = 10;          // «Память №7»

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut();      // из чёрного

        yukiSprite.SetActive(true);             // Юки уже стоит
        goriySprite.SetActive(false);

        /* ---------- реакция Юки на напиток ---------- */
        /* ---------- загружаем JSON, чтобы подтянуть цвет Юки ---------- */
        dialogueManager.StartDialogue("Dialogue_BarDay1_3.json");
        yield return null;                       // один кадр — characterDict готов
        dialogueManager.ShowPanel(false);        // скрываем, чтобы не вспыхнула 1-я строка

        /* ---------- кастомная реакция Юки на напиток ---------- */
        string yukiLine = (GameFlow.CookedDrinkID == favDrinkID)
            ? "Вау! Это же «Память номер 7» — мой любимый напиток!"
            : "Спасибо большое… <i>Юки слегка поморщилась</i>";

        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("yuki", yukiLine);
        yield return WaitForClick();


        /* ---------- ждём Гория ---------- */
        dialogueManager.ShowPanel(false);       // панель скрыта
        goriySprite.SetActive(true);

        float inTime = 0.6f;
        if (goriySprite.TryGetComponent<SpriteEnter>(out var seG))
        {
            seG.EnterFromRight();
            inTime = seG.duration + 0.1f;
        }
        yield return new WaitForSeconds(inTime);

        /* ---------- короткий диалог Юки + Горий (только заказ напитка) ---------- */
        dialogueManager.ShowPanel(true);
        dialogueManager.StartDialogue("Dialogue_BarDay1_3.json");
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        /* ---------- сразу готовим напиток Горию ---------- */
        GameFlow.RequestedDrinkID = -1;           // любой / или проставь нужный ID
        GameFlow.NextSceneName = "BarScene3"; // вернёмся сюда после готовки, если нужно

        yield return screenFade.FadeIn();         // затемнение
        UnityEngine.SceneManagement.SceneManager.LoadScene("CookingScene");
    }

    IEnumerator WaitForClick() =>
        new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BarScene_Day2 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject backgroundBar;
    public GameObject goriySprite; // Горий — появляется справа

    void Start()
    {
        StartCoroutine(StartBarScene());
    }

    IEnumerator StartBarScene()
    {
        // 1. Затемнение
        yield return screenFade.FadeOut();

        // 2. Включаем фон
        backgroundBar.SetActive(true);

        // 3. Мысленная реплика или фоновое вступление (Ёки не видно — она за стойкой)
        dialogueManager.StartDialogue("Dialogue_BarDay2_1.json");
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 1);

        // 4. Появление Гория справа
        goriySprite.SetActive(true);
        goriySprite.GetComponent<SpriteEnter>().EnterFromRight();
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 5. Затемнение перед сценой приготовления
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene1D2";

        SceneManager.LoadScene("CookingScene");
    }
}

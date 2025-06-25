using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BarScene_Day3 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject backgroundBar;
    public GameObject ninaSprite; // Нина — появляется справа

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

        // 3. Мысленная реплика или вступление (Ёки за стойкой, её спрайт не нужен)
        dialogueManager.StartDialogue("Dialogue_BarDay3_1.json");
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 1);

        // 4. Появление Нины справа
        ninaSprite.SetActive(true);
        ninaSprite.GetComponent<SpriteEnter>().EnterFromRight();
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 5. Затемнение перед сценой приготовления
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene1D3";

        SceneManager.LoadScene("CookingScene");
    }
}

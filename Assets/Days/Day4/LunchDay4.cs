using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LunchDay4 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject backgroundImage;
    public GameObject richSprite;
    public GameObject yokaSprite;

    void Start()
    {
        StartCoroutine(RunDaySequence());
    }

    IEnumerator RunDaySequence()
    {
        // 1. Затемнение
        yield return screenFade.FadeOut();

        // 2. Фон
        backgroundImage.SetActive(true);

        // 3. Рич входит справа (он говорит первым)
        richSprite.SetActive(true);
        if (richSprite.TryGetComponent<SpriteEnter>(out var richEnter))
        {
            richEnter.EnterFromRight();
            yield return new WaitForSeconds(richEnter.duration + 0.1f);
        }

        // 4. Ёка входит слева чуть позже
        yokaSprite.SetActive(true);
        if (yokaSprite.TryGetComponent<SpriteEnter>(out var yokaEnter))
        {
            yokaEnter.EnterFromLeft();
            yield return new WaitForSeconds(yokaEnter.duration + 0.1f);
        }

        // 5. Диалоговая панель
        var canvasGroup = dialogueManager.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // 6. Запуск диалога
        dialogueManager.StartDialogue("Dialogue_LunchDay4.json");

        // 7. Дождаться конца диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 8. Уход Ёки влево
        if (yokaSprite.TryGetComponent<SpriteEnter>(out var yokaExit))
        {
            yokaExit.ExitToLeft();
            yield return new WaitForSeconds(yokaExit.duration + 0.1f);
        }
        yokaSprite.SetActive(false);

        // 9. Уход Рича вправо
        if (richSprite.TryGetComponent<SpriteEnter>(out var richExit))
        {
            richExit.ExitToRight();
            yield return new WaitForSeconds(richExit.duration + 0.1f);
        }
        richSprite.SetActive(false);

        // 10. Затемнение → переход в бар
        yield return screenFade.FadeIn();
        SceneManager.LoadScene("BarSceneD4");
    }
}

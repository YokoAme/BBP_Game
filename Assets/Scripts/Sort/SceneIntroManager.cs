using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneIntroManager : MonoBehaviour
{
    [Header("References")]
    public ScreenFade screenFade;
    public SlideInUI paperStackSlide;
    public CanvasGroup gameplayGroup;
    public CanvasGroup tutorialPanel;
    public Button okButton;

    [Header("Settings")]
    public bool showTutorial = true;
    public float gameFade = 0.4f;
    public float panelFade = 0.3f;

    void Awake()
    {
        if (gameplayGroup != null)
        {
            gameplayGroup.alpha = 0;
            gameplayGroup.interactable = false;
            gameplayGroup.blocksRaycasts = false;
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.alpha = 0;
            tutorialPanel.interactable = false;
            tutorialPanel.blocksRaycasts = false;
        }
    }

    IEnumerator Start()
    {
        /* 1. Если открыт туториал – стопку временно скрываем */
        if (showTutorial && paperStackSlide != null)
            paperStackSlide.gameObject.SetActive(false);

        /* 2. Ждём окончания чёрного фейда */
        yield return new WaitForSeconds(screenFade.fadeDuration);

        /* 3. Ветвление: с обучением или без */
        if (showTutorial && tutorialPanel != null)
        {
            yield return StartCoroutine(FadeCanvas(tutorialPanel, 0, 1, panelFade));
            okButton.onClick.AddListener(OnOkClicked);        // ждём «ОК»
        }
        else
        {
            yield return StartCoroutine(FadeCanvas(gameplayGroup, 0, 1, gameFade));
            paperStackSlide.Play();                 // сразу один slide-in
        }
    }

    IEnumerator CloseTutorial()
    {
        /* 1. Убираем панель обучения */
        yield return StartCoroutine(FadeCanvas(tutorialPanel, 1, 0, panelFade));

        /* 2. Проявляем UI интро */
        yield return StartCoroutine(FadeCanvas(gameplayGroup, 0, 1, gameFade));

        /* 3. Возвращаем стопку и ОДИН раз запускаем анимацию */
        paperStackSlide.gameObject.SetActive(true);
        paperStackSlide.Play();
    }


    void OnOkClicked()
    {
        okButton.interactable = false;
        StartCoroutine(CloseTutorial());
    }


    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0f;
        cg.alpha = from;
        cg.interactable = to > 0.99f;
        cg.blocksRaycasts = cg.interactable;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }

        cg.alpha = to;
        cg.interactable = to > 0.99f;
        cg.blocksRaycasts = cg.interactable;
    }

    public void StartEndOfDay()
    {
        StartCoroutine(EndOfDayRoutine());
    }

    IEnumerator EndOfDayRoutine()
    {
        screenFade.gameObject.SetActive(true);
        yield return screenFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    public CanvasGroup settingsPanel; // ← сюда в инспекторе привязать CanvasGroup!

    void Start()
    {


        if (settingsPanel == null)
        {
            Debug.LogError("settingsPanel не привязан!");
            return;
        }

        settingsPanel.alpha = 0f;
        settingsPanel.gameObject.SetActive(false);


        // Установка разрешения 1920x1080 при первом запуске
        if (!PlayerPrefs.HasKey("ResolutionIndex"))
        {
            Resolution[] resolutions = Screen.resolutions;
            int defaultIndex = -1;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == 1920 && resolutions[i].height == 1080)
                {
                    defaultIndex = i;
                    break;
                }
            }

            if (defaultIndex != -1)
            {
                Resolution res = resolutions[defaultIndex];
                Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                PlayerPrefs.SetInt("ResolutionIndex", defaultIndex);
                Debug.Log($"<color=yellow>Установлено разрешение {res.width}×{res.height} по умолчанию</color>");
            }
            else
            {
                Debug.LogWarning("Разрешение 1920x1080 не найдено среди доступных.");
            }
        }
    }


    public void OpenSettings()
    {
        Debug.Log("Открываю настройки...");
        settingsPanel.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(settingsPanel, 0f, 1f, 0.3f));
    }

    public void CloseSettings()
    {
        Debug.Log("Закрываю настройки...");
        StartCoroutine(CloseWithFade());
    }

    IEnumerator CloseWithFade()
    {
        yield return StartCoroutine(FadeCanvasGroup(settingsPanel, 1f, 0f, 0.3f));
        settingsPanel.gameObject.SetActive(false);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float time = 0f;
        cg.alpha = from;
        cg.interactable = to > 0.9f;
        cg.blocksRaycasts = to > 0.9f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        cg.alpha = to;
        cg.interactable = to > 0.9f;
        cg.blocksRaycasts = to > 0.9f;
    }

    public void NewGame()
    {
        Debug.Log("Новая игра: сбрасываем прогресс и запускаем DayTransition");

        if (SaveSystem.Instance != null)
        {
            var data = SaveSystem.Instance.Data;

            data.currentDay = 1;
            data.lastScene = "OfficeScene";  // ← первая игровая сцена
            data.score = 0;              // сброс всех очков
            data.lastScoreCommittedDay = 0;              // ❗ сброс анти-чита

            SaveSystem.Instance.Save();
            Debug.Log("<color=lime>Progress reset: day=1, score=0</color>");
        }
        else
        {
            Debug.LogWarning("❗ SaveSystem.Instance не найден при новой игре");
        }

        SceneManager.LoadScene("DayTransition");
    }


    public void OnContinueClicked()
    {
        Time.timeScale = 1f;
        // если SaveSystem по какой-то причине ещё не создан — создаём
        if (SaveSystem.Instance == null)
            new GameObject("SaveSystem").AddComponent<SaveSystem>();

        // если lastScene пустой – покажи предупреждение и ничего не делай
        if (string.IsNullOrEmpty(SaveSystem.Instance.Data.lastScene))
        {
            Debug.LogWarning("Нет сохранённой сцены — продолжать нечего.");
            return;
        }else SceneManager.LoadScene("DayTransition");
    }


    public void ExitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();

#if UNITY_EDITOR
    // Это работает только в редакторе, при билде автоматически игнорируется
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}

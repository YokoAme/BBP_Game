using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DayTransition : MonoBehaviour
{
    [Header("UI")]
    public ScreenFade screenFade;   // чёрный фейд-канвас
    public TMP_Text dayText;       // надпись «День N»

    [Header("Параметры")]
    public float showTime = 2.5f;          // сколько держать заставку
    public string defaultNextScene = "LunchScene"; // fallback, если сейва нет

    void Start() => StartCoroutine(ShowSequence());

    IEnumerator ShowSequence()
    {
        /* ---------- гарантируем, что SaveSystem существует ---------- */
        if (SaveSystem.Instance == null)
            new GameObject("SaveSystem").AddComponent<SaveSystem>();

        /* ---------- получаем номер дня и сцену ---------- */
        int day = Mathf.Max(1, SaveSystem.Instance.Data.currentDay);
        string nextScene = string.IsNullOrEmpty(SaveSystem.Instance.Data.lastScene)
                         ? defaultNextScene
                         : SaveSystem.Instance.Data.lastScene;

        /* ---------- выводим надпись ---------- */
        dayText.text = $"{day}";

        /* ---------- фейд-ин из чёрного ---------- */
        yield return screenFade.FadeOut();

        /* ---------- держим N секунд ---------- */
        yield return new WaitForSeconds(showTime);

        /* ---------- фейд-аут в чёрное ---------- */
        yield return screenFade.FadeIn();

        /* ---------- загружаем нужную сцену ---------- */
        SceneManager.LoadScene(nextScene);
    }
}

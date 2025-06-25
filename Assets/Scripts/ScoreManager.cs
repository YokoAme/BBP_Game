using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 💡 Удаляем ScoreManager вне сортировки (если не надо сохранять)
        if (!scene.name.StartsWith("SortScene"))
        {
            Destroy(gameObject);
            Instance = null;
        }
    }

    public void RegisterResult(bool isCorrect)
    {
        score += isCorrect ? 1 : -1;
        Debug.Log($"Результат: {(isCorrect ? "Правильно" : "Неправильно")} | Очки за день: {score}");
    }

    public void ResetDayScore()
    {
        score = 0;
        Debug.Log("<color=grey>Очки за день сброшены.</color>");
    }

    public void CommitScoreToSave()
    {
        if (SaveSystem.Instance == null) return;

        int today = SaveSystem.Instance.Data.currentDay;

        // ► если этот день уже записан — просто обнуляем счётчик и выходим
        if (today <= SaveSystem.Instance.Data.lastScoreCommittedDay)
        {
            ResetDayScore();
            Debug.Log("<color=grey>Очки за этот день уже занесены. Повтор не считается.</color>");
            return;
        }

        // ► первый (и единственный) раз за день
        SaveSystem.Instance.Data.score += score;                 // прибавляем
        SaveSystem.Instance.Data.lastScoreCommittedDay = today;  // отмечаем день
        SaveSystem.Instance.Save();

        Debug.Log($"<color=yellow>Добавлено {score} очков. Всего: {SaveSystem.Instance.Data.score}</color>");

        ResetDayScore(); // обнуляем локальный счётчик
    }

}

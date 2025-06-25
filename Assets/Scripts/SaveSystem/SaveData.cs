// SaveData.cs  — единый контейнер данных, которые пишутся в save.json
[System.Serializable]
public class SaveData
{
    /* прогресс ------------------------------------------------------- */
    public string lastScene = "Intro";  // сцена, с которой продолжить
    public int currentDay = 1;       // номер дня/главы
    public int score = 0;            // суммарные очки за досье
    public int lastScoreCommittedDay = 0;   // ← новый: день, за который очки уже учтены

    public bool hasBossKey = false;

    /* настройки ------------------------------------------------------- */
    public float masterVolume = 1f;
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
    public bool fullscreen = true;
    public int resolutionIndex = 0;   // индекс в Screen.resolutions
}

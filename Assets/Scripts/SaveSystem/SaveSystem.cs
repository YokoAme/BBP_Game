// SaveSystem.cs  Ч отвечает за чтение / запись JSON
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    public SaveData Data { get; private set; }

    string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    /* ---------- инициализаци€ ---------- */
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();                 // загружаем файл при старте
        }
        else Destroy(gameObject);
    }

    /* ---------- API ---------- */
    public void Save()
    {
        string json = JsonUtility.ToJson(Data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (File.Exists(SavePath))
            Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        else
            Data = new SaveData();      // дефолт, если файла нет
    }
}

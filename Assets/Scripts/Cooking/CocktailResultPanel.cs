using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CocktailResultPanel : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private Image cocktailPic;
    [SerializeField] private TMP_Text cocktailName;
    [SerializeField] private Button btnServe;
    [SerializeField] private Button btnAgain;

    [Header("Карты спрайтов (необязательно)")]
    public List<CocktailIDSprite> spriteByID;      // ручная таблица  ID → Sprite
    [SerializeField] private Sprite unknownSprite; // fallback

    private Dictionary<int, Sprite> idLookup;
    private int currentDrinkID;                    // ← сюда пишем выбранный напиток

    /* ─────────────────────────────── */

    void Awake()
    {
        // строим lookup-словарь
        idLookup = new Dictionary<int, Sprite>();
        foreach (var item in spriteByID)
            idLookup[item.id] = item.sprite;

        btnServe.onClick.AddListener(Serve);
        btnAgain.onClick.AddListener(Hide);

        Hide();  // стартовая невидимость
    }

    /* ---------- Публичные методы ---------- */

    /// <summary>Показать окно результата по ID (и подписью имени).</summary>
    public void Show(int id, string displayName)
    {
        Debug.Log($"[ResultPanel] Show called with id = {id}");

        gameObject.SetActive(true);
        cocktailName.text = displayName;
        currentDrinkID = id; // сохраняем текущий ID

        // 1) ищем в ручном справочнике
        if (idLookup.TryGetValue(id, out var spManual))
        {
            cocktailPic.sprite = spManual;
            return;
        }

        // 2) пробуем загрузить из Resources/Drinks/ID
        Sprite auto = Resources.Load<Sprite>($"Drinks/{id}");
        cocktailPic.sprite = auto ? auto : unknownSprite;
    }

    public void Hide() => gameObject.SetActive(false);

    /* ---------- Кнопка «Подать» ---------- */
    void Serve()
    {
        Debug.Log($"[ResultPanel] Serve: записываем CookedID = {currentDrinkID}");

        GameFlow.CookedDrinkID = currentDrinkID;   // сохраняем ID как int

        // Переходим в указанную сцену
        string next = GameFlow.NextSceneName;
        if (!string.IsNullOrEmpty(next))
            SceneManager.LoadScene(next);
        else
            Debug.LogWarning("Next scene is not set!");
    }

    /* ---------- Вспом. структура ---------- */
    [System.Serializable]
    public struct CocktailIDSprite
    {
        public int id;
        public Sprite sprite;
    }
}
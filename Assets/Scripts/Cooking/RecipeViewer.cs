using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeViewer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image recipeImage;
    [SerializeField] private Button btnPrev;
    [SerializeField] private Button btnNext;

    [Header("Спрайты из папки Menu")]
    [SerializeField] private List<Sprite> recipeSprites;  // заполняем в инспекторе

    private int currentIndex = 0;

    void Start()
    {
        btnPrev.onClick.AddListener(Prev);
        btnNext.onClick.AddListener(Next);
        ShowCurrent();
    }

    void Prev()
    {
        if (recipeSprites.Count == 0) return;
        currentIndex = (currentIndex - 1 + recipeSprites.Count) % recipeSprites.Count;
        ShowCurrent();
    }

    void Next()
    {
        if (recipeSprites.Count == 0) return;
        currentIndex = (currentIndex + 1) % recipeSprites.Count;
        ShowCurrent();
    }

    void ShowCurrent()
    {
        recipeImage.sprite = recipeSprites[currentIndex];
    }
}

using UnityEngine;
using System.Collections;

public class BarScene3_Day3 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject ninaSprite;
    public GameObject mishaSprite;

    private readonly int[] acceptedDrinks = { 11, 15 };

    void Start() => StartCoroutine(RunScene());

    IEnumerator RunScene()
    {
        yield return screenFade.FadeOut(); // затемнение

        ninaSprite.SetActive(true);
        mishaSprite.SetActive(false);

        // Подгружаем JSON, чтобы подгрузить characterDict
        dialogueManager.StartDialogue("Dialogue_BarDay3_3.json");
        yield return null;
        dialogueManager.ShowPanel(false);

        // Проверка напитка
        int cooked = GameFlow.CookedDrinkID;
        bool isCorrect = System.Array.Exists(acceptedDrinks, id => id == cooked);

        string ninaLine = isCorrect
            ? "Вот это я понимаю, вот это прямо в точку моего настроения."
            : "Ну и что это такое, совсем не то что я хотела. Платить не буду.";

        // Первая кастомная реплика Нины
        dialogueManager.ShowPanel(true);
        dialogueManager.ShowCustomLine("nina", ninaLine);
        yield return WaitForClick();

        // Запускаем основной диалог
        dialogueManager.StartDialogue("Dialogue_BarDay3_3.json");

        // Ждём, пока дойдём до реплики 5 (index 5)
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 5);
        dialogueManager.dialoguePanel.interactable = false;
        yield return new WaitForSeconds(0.6f);

        // Прячем панель и уводим Нину
        dialogueManager.ShowPanel(false);
        if (ninaSprite.TryGetComponent<SpriteEnter>(out var ninaEnter))
        {
            ninaEnter.ExitToRight();
            yield return new WaitForSeconds(ninaEnter.duration + 0.1f);
        }
        ninaSprite.SetActive(false);

        // Пауза и возврат панели (без ForceNextLine)
        yield return new WaitForSeconds(1.0f);
        dialogueManager.ShowPanel(true);
        dialogueManager.dialoguePanel.interactable = true;

        // Игрок вручную продолжает диалог — реплика Ёки
        yield return WaitForClick();

        // Ждём до конца реплики Ёки, например index >= 6
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 6);

        // Небольшая пауза перед входом Миши
        yield return new WaitForSeconds(0.5f);

        // Входит Миша
        mishaSprite.SetActive(true);
        if (mishaSprite.TryGetComponent<SpriteEnter>(out var mishaEnter))
        {
            mishaEnter.EnterFromRight();
            yield return new WaitForSeconds(mishaEnter.duration + 0.1f);
        }

        // Панель уже активна, продолжаем диалог вручную — по кликам игрока
        yield return WaitForClick();

        // Ждём завершения диалога
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene3D3";
        UnityEngine.SceneManagement.SceneManager.LoadScene("CookingScene");
    }

    IEnumerator WaitForClick() =>
        new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
}

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DayTransition : MonoBehaviour
{
    [Header("UI")]
    public ScreenFade screenFade;   // ������ ����-������
    public TMP_Text dayText;       // ������� ����� N�

    [Header("���������")]
    public float showTime = 2.5f;          // ������� ������� ��������
    public string defaultNextScene = "LunchScene"; // fallback, ���� ����� ���

    void Start() => StartCoroutine(ShowSequence());

    IEnumerator ShowSequence()
    {
        /* ---------- �����������, ��� SaveSystem ���������� ---------- */
        if (SaveSystem.Instance == null)
            new GameObject("SaveSystem").AddComponent<SaveSystem>();

        /* ---------- �������� ����� ��� � ����� ---------- */
        int day = Mathf.Max(1, SaveSystem.Instance.Data.currentDay);
        string nextScene = string.IsNullOrEmpty(SaveSystem.Instance.Data.lastScene)
                         ? defaultNextScene
                         : SaveSystem.Instance.Data.lastScene;

        /* ---------- ������� ������� ---------- */
        dayText.text = $"{day}";

        /* ---------- ����-�� �� ������� ---------- */
        yield return screenFade.FadeOut();

        /* ---------- ������ N ������ ---------- */
        yield return new WaitForSeconds(showTime);

        /* ---------- ����-��� � ������ ---------- */
        yield return screenFade.FadeIn();

        /* ---------- ��������� ������ ����� ---------- */
        SceneManager.LoadScene(nextScene);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class BarScene_Day1 : MonoBehaviour
{
    public ScreenFade screenFade;
    public DialogueManager dialogueManager;
    public GameObject backgroundBar;
    public GameObject shepsSprite;
    public GameObject yokaSprite;

    void Start()
    {
        StartCoroutine(StartBarScene());
    }

    IEnumerator StartBarScene()
    {
        // 1. ����������
        yield return screenFade.FadeOut();

        // 2. �������� ���
        backgroundBar.SetActive(true);

        // 3. ��������� ������� (��� ��� �� �����)
        yokaSprite.SetActive(false);
        dialogueManager.StartDialogue("Dialogue_BarDay1_1.json");
        yield return new WaitUntil(() => dialogueManager.CurrentLineIndex >= 1);

        // 4. ��������� ����� ������
        shepsSprite.SetActive(true);
        shepsSprite.GetComponent<SpriteEnter>().EnterFromRight();
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished);

        // 5. ���������� ����� ������ �������������
        yield return screenFade.FadeIn();
        GameFlow.NextSceneName = "BarScene1";

        SceneManager.LoadScene("CookingScene");
    }
}

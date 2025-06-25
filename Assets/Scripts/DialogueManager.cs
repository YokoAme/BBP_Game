using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class CharacterData
{
    public string key;
    public string name;
    public string color;
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

[System.Serializable]
public class DialogueFile
{
    public CharacterData[] characters;
    public DialogueLine[] dialogue;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public CanvasGroup dialoguePanel;

    [Header("Dialogue Settings")]
    public string dialogueFileName;
    public float typingSpeed = 0.04f;
    public float fadeDuration = 0.3f;

    private Dictionary<string, CharacterData> characterDict;
    private DialogueLine[] dialogueLines;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    public int CurrentLineIndex => currentIndex;
    public bool IsDialogueFinished => dialogueLines != null && currentIndex >= dialogueLines.Length;
    private bool isCustomLineActive = false;
    private string customSpeaker = "";
    private string customText = "";

    public void StartDialogue(string fileName)
    {
        isCustomLineActive = false;
        dialogueFileName = fileName;
        LoadDialogue(dialogueFileName);
        ShowPanel(true);
        ShowLine();
    }

    public void ForceNextLine()
    {
        NextLine();
    }

    void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.interactable) return;

        bool space = Input.GetKeyDown(KeyCode.Space);
        bool click = Input.GetMouseButtonDown(0);

        // ► Игнорируем клики, если курсор выше 40 % экрана
        if (click && Input.mousePosition.y > Screen.height * 0.3f)
            click = false;

        if (!space && !click) return;

        // ───────── стандартная логика ─────────
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = isCustomLineActive ? customText
                                                   : dialogueLines[currentIndex].text;
            isTyping = false;
        }
        else if (!isCustomLineActive)
        {
            NextLine();
        }
    }



    void LoadDialogue(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DialogueFile dialogueData = JsonUtility.FromJson<DialogueFile>(json);

            characterDict = new Dictionary<string, CharacterData>();
            foreach (var c in dialogueData.characters)
                characterDict[c.key] = c;

            dialogueLines = dialogueData.dialogue;
            currentIndex = 0;
        }
        else
        {
            Debug.LogError("Файл диалога не найден: " + path);
        }
    }

    void ShowLine()
    {
        if (dialogueLines == null || currentIndex >= dialogueLines.Length)
        {
            ShowPanel(false);
            return;
        }

        DialogueLine line = dialogueLines[currentIndex];
        ApplyCharacter(line.speaker);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        int i = 0;
        string fullText = "";

        while (i < text.Length)
        {
            if (text[i] == '<')
            {
                int tagEnd = text.IndexOf('>', i);
                if (tagEnd != -1)
                {
                    string tag = text.Substring(i, tagEnd - i + 1);
                    fullText += tag;
                    i = tagEnd + 1;
                    continue;
                }
            }

            fullText += text[i];
            dialogueText.text = fullText;
            i++;

            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueText.text = fullText;
        isTyping = false;
    }

    void NextLine()
    {
        currentIndex++;
        ShowLine();
    }

    public void ShowPanel(bool show)
    {
        if (dialoguePanel != null)
            StartCoroutine(FadePanel(show));
    }

    IEnumerator FadePanel(bool fadeIn)
    {
        float start = dialoguePanel.alpha;
        float end = fadeIn ? 1f : 0f;
        float time = 0f;

        dialoguePanel.interactable = fadeIn;
        dialoguePanel.blocksRaycasts = fadeIn;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            dialoguePanel.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        dialoguePanel.alpha = end;
    }

    void ApplyCharacter(string speakerKey)
    {
        if (characterDict != null && characterDict.TryGetValue(speakerKey, out CharacterData ch))
        {
            nameText.text = ch.name;
            if (ColorUtility.TryParseHtmlString(ch.color, out Color col))
                nameText.color = col;
            else
                nameText.color = Color.white;
        }
        else
        {
            nameText.text = speakerKey;
            nameText.color = Color.white;
        }
    }

    public void ShowCustomLine(string speakerKey, string text)
    {
        isCustomLineActive = true;
        customSpeaker = speakerKey;
        customText = text;

        ApplyCharacter(speakerKey);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }
}

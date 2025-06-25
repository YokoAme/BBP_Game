using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsPanelController : MonoBehaviour
{
    public CanvasGroup settingsPanel;

    private bool isOpen = false;

    void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.alpha = 0f;
            settingsPanel.interactable = false;
            settingsPanel.blocksRaycasts = false;
            settingsPanel.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen)
                OpenSettings();
            else
                CloseSettings();
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;



        settingsPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;

        settingsPanel.alpha = 1f;
        settingsPanel.interactable = true;
        settingsPanel.blocksRaycasts = true;
        isOpen = true;
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.alpha = 0f;
        settingsPanel.interactable = false;
        settingsPanel.blocksRaycasts = false;
        settingsPanel.gameObject.SetActive(false);
        isOpen = false;

        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenuScene");
    }
}

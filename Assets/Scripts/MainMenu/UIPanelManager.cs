using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIPanelManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject docsPanel;

    [Header("UI элементы настроек")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] basicResolutions = new Resolution[]
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1366, height = 768 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 }
    };

    void Start()
    {
        // === Громкость ===
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // === Полноэкранный режим ===
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;

        // === Разрешения ===
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int defaultIndex = 3; // 1920x1080 — по умолчанию
        int currentIndex = defaultIndex;

        for (int i = 0; i < basicResolutions.Length; i++)
        {
            var r = basicResolutions[i];
            string label = r.width + " x " + r.height;
            options.Add(label);

            if (PlayerPrefs.HasKey("ResolutionIndex") &&
                PlayerPrefs.GetInt("ResolutionIndex") == i)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        // Не применяем сразу
    }




    public void ApplySettings()
    {
        // 1. Разрешение
        int idx = resolutionDropdown.value;
        var res = basicResolutions[idx];

        bool wantFullscreen = fullscreenToggle.isOn;

        Screen.SetResolution(res.width, res.height, wantFullscreen);

        // 2. Громкость
        float vol = volumeSlider.value;
        AudioListener.volume = vol;

        // 3. Сохраняем всё в PlayerPrefs
        PlayerPrefs.SetInt("ResolutionIndex", idx);
        PlayerPrefs.SetInt("Fullscreen", wantFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", vol);
        PlayerPrefs.Save();


    }


    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ShowSettings() => settingsPanel.SetActive(true);

    public void ShowHelp() => helpPanel.SetActive(true);
    public void HideHelp() => helpPanel.SetActive(false);

    public void ShowDocs() => docsPanel.SetActive(true);
    public void HideDocs() => docsPanel.SetActive(false);
}

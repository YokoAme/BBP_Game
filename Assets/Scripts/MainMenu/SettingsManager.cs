using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Слайдеры громкости")]
    public Slider sfxVolume;
    public Slider musicVolume;
    public Slider masterVolume;

    [Header("Экран")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    [Header("Кнопка Назад")]
    public Button btnBack;

    Resolution[] resolutions;

    void Start()
    {
        // Назначаем кнопку назад
        btnBack.onClick.AddListener(CloseSettings);

        // Громкости
        sfxVolume.value = PlayerPrefs.GetFloat("SfxVolume", 1);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 1);

        sfxVolume.onValueChanged.AddListener((v) => PlayerPrefs.SetFloat("SfxVolume", v));
        musicVolume.onValueChanged.AddListener((v) => PlayerPrefs.SetFloat("MusicVolume", v));
        masterVolume.onValueChanged.AddListener((v) => PlayerPrefs.SetFloat("MasterVolume", v));

        // Полный экран
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.onValueChanged.AddListener((isFull) =>
        {
            Screen.fullScreen = isFull;
            PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
        });

        // Разрешения
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string opt = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(opt);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", currentIndex);
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", index);
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false); // Скрываем панель
    }
}

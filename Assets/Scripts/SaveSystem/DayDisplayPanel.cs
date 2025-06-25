using UnityEngine;
using TMPro;

public class DayDisplayPanel : MonoBehaviour
{
    public TMP_Text dayText;

    void Start()
    {
        if (SaveSystem.Instance == null || SaveSystem.Instance.Data == null)
        {
            Debug.LogWarning("SaveSystem.Instance.Data не загружен");
            dayText.text = "День ?";
            return;
        }

        int day = SaveSystem.Instance.Data.currentDay;
        dayText.text = $"{day}";
    }
}

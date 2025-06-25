using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;            // сюда вставляешь звук
    public AudioSource audioSource;


    void Start()
    {
        audioSource = GameObject.Find("UI_SoundPlayer").GetComponent<AudioSource>();
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, PlayerPrefs.GetFloat("Volume", 1f));
        }
    }
}

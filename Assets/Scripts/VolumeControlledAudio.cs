using UnityEngine;

public class VolumeControlledAudio : MonoBehaviour
{
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        source.volume = savedVolume;
    }

    void Update()
    {
        // На случай, если громкость изменилась во время работы
        source.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }
}

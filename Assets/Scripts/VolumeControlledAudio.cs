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
        // �� ������, ���� ��������� ���������� �� ����� ������
        source.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }
}

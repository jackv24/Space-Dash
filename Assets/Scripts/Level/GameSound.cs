using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameSound : MonoBehaviour
{
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        SoundManager.instance.OnGameVolumeChanged += UpdateVolume;

        if (source)
            source.volume = SoundManager.instance.gameVolume;
    }

    void UpdateVolume(float volume)
    {
        if (source)
            source.volume = volume;
    }

    void OnDestroy()
    {
        SoundManager.instance.OnGameVolumeChanged -= UpdateVolume;
    }
}

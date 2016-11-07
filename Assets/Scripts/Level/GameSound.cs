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
    }

    void UpdateVolume(float volume)
    {
        source.volume = volume;
    }
}

using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Music")]
    [Tooltip("The background music to play looping.")]
    public AudioClip backgroundMusic;
    [Range(0, 1f)]
    public float musicVolume = 1f;

    [Range(0, 1f)]
    public float gameVolume = 1f;

    private AudioSource source;

    void Awake()
    {
        if (instance)
            Debug.LogWarning("More than one SoundManager in scene. There should only ever be one SoundManager present!");
        else
            instance = this;

        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        //Play looping background music on start
        source.loop = true;
        source.clip = backgroundMusic;
        source.volume = musicVolume;
        source.Play();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        source.volume = musicVolume;
    }

    public void SetGameVolume(float value)
    {
        gameVolume = value;
    }
}

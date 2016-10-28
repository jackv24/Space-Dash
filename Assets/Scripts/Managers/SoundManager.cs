using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Music")]
    [Tooltip("The background music to play looping, randomly selected.")]
    public AudioClip[] backgroundMusic;
    [Range(0, 1f)]
    public float musicVolume = 1f;

    public AudioSource musicSource;

    [Header("Game")]
    [Range(0, 1f)]
    public float gameVolume = 1f;

    public AudioSource gameSource;

    void Awake()
    {
        if (instance)
            Debug.LogWarning("More than one SoundManager in scene. There should only ever be one SoundManager present!");
        else
            instance = this;
    }

    void Start()
    {
        //Play looping background music on start
        StartBackgroundMusic();
    }

    public void StartBackgroundMusic()
    {
        musicSource.loop = true;
        musicSource.clip = backgroundMusic[Random.Range(0, backgroundMusic.Length)];
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
    }

    public void SetGameVolume(float value)
    {
        gameVolume = value;
    }
}

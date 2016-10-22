using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("The background music to play looping.")]
    public AudioClip backgroundMusic;
    [Range(0, 1f)]
    public float musicVolume = 1f;

    private AudioSource source;

    void Awake()
    {
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
}

using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public delegate void VolumeChangedEvent(float volume);
    public VolumeChangedEvent OnGameVolumeChanged;

    [Header("Music")]
    [Range(0, 1f)]
    public float musicVolume = 1f;
    public AudioSource musicSource;
    [Tooltip("The background music to play looping, randomly selected.")]
    public AudioClip[] backgroundMusic;

    [Header("Game")]
    [Range(0, 1f)]
    public float gameVolume = 1f;
    public AudioSource gameSource;
    public AudioSource playerLoopSource;

    [System.Serializable]
    public class GameSounds
    {
        [System.Serializable]
        public class Clip
        {
            public AudioClip clip;
            public float volume = 1f;
        }

        public Clip running;

        public Clip[] jumps;
        public Clip RandomJump { get { return jumps[Random.Range(0, jumps.Length)]; } }

        public Clip landing;
        public Clip boosting;
        public Clip pickupOxygen;
        public Clip pickupOxygenPowerup;
        public Clip pickupJumpPowerup;

        public Clip newHighScore;

        public Clip[] deaths;
        public Clip RandomDeath { get { return deaths[Random.Range(0, deaths.Length)]; } }
    }
    public GameSounds sounds;

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

    public void PlaySound(GameSounds.Clip clip)
    {
        if (clip != null)
            gameSource.PlayOneShot(clip.clip, clip.volume);
    }

    public void SetPlayerLoop(GameSounds.Clip clip)
    {
        if (clip == null)
        {
            playerLoopSource.Stop();
            return;
        }

        playerLoopSource.clip = clip.clip;
        playerLoopSource.volume = clip.volume * gameVolume;
        playerLoopSource.Play();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
    }

    public void SetGameVolume(float value)
    {
        gameVolume = value;
        gameSource.volume = value;
        playerLoopSource.volume = value;

        //Call events from game audio source scripts
        if(OnGameVolumeChanged != null)
            OnGameVolumeChanged(value);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Music")]
    public AudioSource musicSource;
    [Tooltip("The background music to play looping, randomly selected.")]
    public AudioClip[] backgroundMusic;

    [Header("Game")]
    public AudioSource gameSource;
    public AudioSource playerLoopSource;
    public AudioSource warningSource;

    [System.Serializable]
    public class GameSounds
    {
        [System.Serializable]
        public class Clip
        {
            public AudioClip clip;
            [Range(0, 1f)]
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
        public Clip chainBonus;

        public Clip newHighScore;

        public Clip lowOxygen;

        public Clip[] deaths;
        public Clip RandomDeath { get { return deaths[Random.Range(0, deaths.Length)]; } }

        public Clip buttonClick;
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

    void Update()
    {
        //Play sound whenever a button is clicked
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null)
        {
            PlaySound(sounds.buttonClick);
        }
    }

    public void StartBackgroundMusic()
    {
        musicSource.loop = true;
        musicSource.clip = backgroundMusic[Random.Range(0, backgroundMusic.Length)];
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
        playerLoopSource.volume = clip.volume;
        playerLoopSource.Play();
    }    

    public void PlayWarning(bool shouldPlay)
    {
        if (warningSource && sounds.lowOxygen != null)
        {
            warningSource.volume = sounds.lowOxygen.volume;

            warningSource.clip = sounds.lowOxygen.clip;

            if (shouldPlay)
                warningSource.Play();
            else
                warningSource.Stop();
        }
    }
}

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Uses read-only property to ensure correct functions are called to start and stop game
    private bool isGamePlaying = false;
    public bool IsGamePlaying { get { return isGamePlaying; } }

    private bool isGamePaused = false;
    public bool IsGamePaused
    {
        get { return isGamePaused; }
        set
        {
            isGamePaused = value;

            if (isGamePaused)
            {
                if (OnGamePaused != null)
                    OnGamePaused();
            }
            else
            {
                if (OnGameResumed != null)
                    OnGameResumed();
            }
        }
    }

    public delegate void Event();
    public event Event OnGamePaused;
    public event Event OnGameResumed;

    void Awake()
    {
        if (instance)
            Debug.LogWarning("More than one GameManager in scene. There should only ever be one GameManager present!");
        else
            instance = this;

        DebugInfo.displayDebugInfo = Debug.isDebugBuild;
    }

    void Start()
    {
        TransitionImageEffect effect = Camera.main.GetComponent<TransitionImageEffect>();

        //Transition only fades in
        effect.playStart = false;
        effect.playEnd = true;
        effect.cutoffAmount = 1f;

        //Play transition on start
        effect.PlayTransition();

        //Reset value for later death transitions
        effect.playStart = true;
    }

    void Update()
    {
        if (DebugInfo.displayDebugInfo)
            DebugInfo.framesPerSecond = 1 / Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
            DebugInfo.displayDebugInfo = !DebugInfo.displayDebugInfo;
    }

    //Ensures that all inputs before game starts aren't carried into game start
    void LateUpdate()
    {
        if (!isGamePlaying && Input.anyKeyDown)
            StartGame();
    }

    public void StartGame()
    {
        isGamePlaying = true;
    }

    public void StopGame()
    {
        isGamePlaying = false;
    }

    public void ResetGame()
    {
        if (LevelGenerator.instance)
            LevelGenerator.instance.Reset();

        //Randomise background on death
        if (BackgroundManager.instance)
            BackgroundManager.instance.Randomise();

        if (SoundManager.instance)
            SoundManager.instance.StartBackgroundMusic();
    }
}
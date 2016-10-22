using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Uses read-only property to ensure correct functions are called to start and stop game
    private bool isGamePlaying = false;
    public bool IsGamePlaying { get { return isGamePlaying; } }

    void Awake()
    {
        if (instance)
            Debug.LogWarning("More than one GameManager in scene. There should only ever be one GameManager present!");
        else
            instance = this;
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
}

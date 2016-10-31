using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public static HUDControl instance;

    [Tooltip("The transform of the player. The x coordinate of this transform is the \"distance travelled\".")]
    public Transform player;
    private PlayerStats playerStats;
    private PlayerControl playerControl;

    [Header("HUD Elements")]
    [Tooltip("The text object to display distance.")]
    public Text distanceText;
    private string distanceTextString;

    private float bestDistance;

    [Space()]
    [Tooltip("The slider to display oxygen level.")]
    public Slider oxygenSlider;
    [Tooltip("The fill image for the slider, of which to change the colour.")]
    public Image barImage;
    [Tooltip("The gradient from which to colour the bar.")]
    public Gradient barColour;

    [Space()]
    [Tooltip("The text to be used to display oxygen level.")]
    public Text oxygenText;
    private string oxygenTextString;

    [Space()]
    [Tooltip("The text object to display score.")]
    public Text scoreText;
    private string scoreTextString;
    public Text highScoreText;

    [Space()]
    public GameObject jumpsPanel;
    private List<Image> jumpsIcons = new List<Image>();
    private int jumpAmount = 0;

    private int bestScore;

    [Header("Game Flow")]
    public Text startText;
    public float fadeDuration = 0.25f;

    void Awake()
    {
        if (instance)
            Debug.LogWarning("More than one HUDControl in scene. There should only ever be one HUDControl present!");
        else
            instance = this;
    }

    void Start()
    {
        //If no player is assigned, attempt to find one
        if (!player)
            player = GameObject.FindWithTag("Player").transform;

        //Cache strings for formatting
        if (distanceText)
            distanceTextString = distanceText.text;
        if (oxygenText)
            oxygenTextString = oxygenText.text;
        if (scoreText)
            scoreTextString = scoreText.text;

        //Load data
        bestDistance = PlayerPrefs.GetFloat("BestDistance", 0);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        //Get PlayerStats from player
        if (player)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerControl = player.GetComponent<PlayerControl>();
        }

        if (jumpsPanel)
            UpdateJumpAmount();
    }

    void Update()
    {
        //Check that player is assigned
        if (player)
        {
            if (player.position.x > bestDistance)
                bestDistance = player.position.x;

            if (highScoreText)
            {
                //Show high score text if high score has been passed
                if (playerStats.Score >= bestScore)
                    highScoreText.gameObject.SetActive(true);
                else if (highScoreText)
                    highScoreText.gameObject.SetActive(false);
            }

            //Check that distancetext is assigned
            if (distanceText)
            {
                //Plug distance into string, using original string's formatting
                distanceText.text = string.Format(distanceTextString, player.position.x, bestDistance);
            }

            //If there is an oxygen slider
            if (oxygenSlider)
            {
                //Display ratio between current and max oxygen (cast to float so that result is a float)
                oxygenSlider.value = (float)playerStats.currentOxygen / playerStats.maxOxygen;

                if (barImage)
                    barImage.color = barColour.Evaluate(oxygenSlider.value);
            }

            //If there is an oxygen text component assigned
            if (oxygenText)
                //Display current and max oxygen in formatted string
                oxygenText.text = string.Format(oxygenTextString, playerStats.currentOxygen, playerStats.maxOxygen);

            if (scoreText)
            {
                //Plug distance into string, using original string's formatting
                scoreText.text = string.Format(scoreTextString, playerStats.Score, bestScore);
            }

            if (jumpsPanel)
            {
                //Loop through all jump icons
                for(int i = 0; i < jumpsIcons.Count; i++)
                {
                    //Set animation based on if that jump is available or not
                    if (playerControl.jumpsLeft > i)
                        jumpsIcons[i].GetComponent<Animator>().SetBool("isFull", true);
                    else
                        jumpsIcons[i].GetComponent<Animator>().SetBool("isFull", false);
                }

                if(jumpAmount != playerControl.jumpAmount)
                    UpdateJumpAmount();
            }

            //Hide and show start game text
            if (GameManager.instance.IsGamePlaying)
                startText.CrossFadeAlpha(0f, fadeDuration, false);
            else
                startText.CrossFadeAlpha(1f, fadeDuration, false);
        }
        else
            Debug.Log("No player transform assigned to HUDControl");
    }

    void UpdateJumpAmount()
    {
        //Get the existing jump icon to copy
        GameObject icon = jumpsPanel.transform.GetChild(0).gameObject;
        icon.SetActive(false);

        //Create any copies needed
        for (int i = jumpsIcons.Count; i < playerControl.jumpAmount; i++)
        {
            GameObject n = (GameObject)Instantiate(icon, jumpsPanel.transform);
            jumpsIcons.Add(n.GetComponent<Image>());
            n.SetActive(true);
        }

        jumpAmount = jumpsIcons.Count;
    }
}

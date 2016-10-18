using UnityEngine;
using System.Collections;
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

    [Space()]
    public Text jumpText;
    private string jumpTextString;

    private int bestScore;

    void Awake()
    {
        //There should only ever be one HUD controller
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
        if (jumpText)
            jumpTextString = jumpText.text;

        //Load data
        bestDistance = PlayerPrefs.GetFloat("BestDistance", 0);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        //Get PlayerStats from player
        if (player)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerControl = player.GetComponent<PlayerControl>();
        }
    }

    void Update()
    {
        //Check that player is assigned
        if (player)
        {
            if (player.position.x > bestDistance)
                bestDistance = player.position.x;

            if (playerStats.Score > bestScore)
                bestScore = playerStats.Score;

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

            if (jumpText)
                jumpText.text = string.Format(jumpTextString, (playerControl.isFloating) ? "Floating!" : playerControl.jumpsLeft.ToString());
        }
        else
            Debug.Log("No player transform assigned to GameManager");
    }
}

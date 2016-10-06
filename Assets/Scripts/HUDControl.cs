using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public static HUDControl instance;

    [Tooltip("The transform of the player. The x coordinate of this transform is the \"distance travelled\".")]
    public Transform player;
    private PlayerStats playerStats;

    [Header("HUD Elements")]
    [Tooltip("The text object to display distance.")]
    public Text distanceText;
    private string distanceTextString;

    private float bestDistance;

    [Space()]
    [Tooltip("The slider to display oxygen level.")]
    public Slider oxygenSlider;
    public Text oxygenText;
    private string oxygenTextString;

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

        //Cache the distanceText string for formatting (if it is assigned)
        if (distanceText)
            distanceTextString = distanceText.text;
        //Cache oxygenText string for formatting
        if (oxygenText)
            oxygenTextString = oxygenText.text;

        //Load data
        bestDistance = PlayerPrefs.GetFloat("BestDistance", 0);

        //Get PlayerStats from player
        if (player)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        //Check that player is assigned
        if (player)
        {
            if (player.position.x > bestDistance)
                bestDistance = player.position.x;

            //Check that distancetext is assigned
            if (distanceText)
            {
                //Plug distance into string, using original string's formatting
                distanceText.text = string.Format(distanceTextString, player.position.x, bestDistance);
            }

            //If there is an oxygen slider
            if(oxygenSlider)
                //Display ratio between current and max oxygen (cast to float so that result is a float)
                oxygenSlider.value = (float)playerStats.currentOxygen / playerStats.maxOxygen;

            //If there is an oxygen text component assigned
            if (oxygenText)
                //Display current and max oxygen in formatted string
                oxygenText.text = string.Format(oxygenTextString, playerStats.currentOxygen, playerStats.maxOxygen);
        }
        else
            Debug.Log("No player transform assigned to GameManager");
    }

    //Called when the player dies, used for saving HUD data after a run
    public void SaveData()
    {
        //Save data
        PlayerPrefs.SetFloat("BestDistance", bestDistance);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Tooltip("The transform of the player. The x coordinate of this transform is the \"distance travelled\".")]
    public Transform player;

    [Header("HUD Elements")]
    [Tooltip("The text object to display distance.")]
    public Text distanceText;
    private string distanceTextString;

    private float bestDistance;

    void Awake()
    {
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

        //Load data
        bestDistance = PlayerPrefs.GetFloat("BestDistance", 0);
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
        }
        else
            Debug.Log("No player transform assigned to GameManager");
    }

    public void EndRun()
    {
        //Save data
        PlayerPrefs.SetFloat("BestDistance", bestDistance);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RandomTextOnStart : MonoBehaviour
{
    [Tooltip("A random string from this list will be chosen on start.")]
    public string[] possibleStrings;

    private PlayerCharStats playerStats;

    private Text text;
    private string formatString;

    void Start()
    {
        //If a player exists, get a reference to its playerstats
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
            playerStats = player.GetComponent<PlayerCharStats>();

        //Randomise text when game resets
        if (playerStats)
            playerStats.OnReset += RandomiseText;

        //If there is a text component, cache it's text for formatting, else show a warning
        text = GetComponent<Text>();
        if (text)
            formatString = text.text;
        else
            Debug.LogWarning("The GameObject that this script is attached to has no Text component!");

        //Randomise text on first start
        RandomiseText();
    }

    void RandomiseText()
    {
        if (text && possibleStrings.Length > 0)
        {
            text.text = string.Format(formatString, possibleStrings[Random.Range(0, possibleStrings.Length)]);
        }
    }
}

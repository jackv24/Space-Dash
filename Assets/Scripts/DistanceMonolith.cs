using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DistanceMonolith : MonoBehaviour
{
    public Text distanceText;

    void Start()
    {
        if (distanceText)
        {
            float bestDistance = PlayerPrefs.GetFloat("BestDistance", 0f);

            //Set distance text
            distanceText.text = string.Format(distanceText.text, bestDistance);

            //Put monolith in exact position that player has made
            Vector3 pos = transform.position;
            pos.x = bestDistance;
            transform.position = pos;
        }
    }
}

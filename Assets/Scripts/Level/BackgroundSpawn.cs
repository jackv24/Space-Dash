using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundSpawn : MonoBehaviour
{
    public float minOffset = 5f;
    public float maxOffset = 20f;
    private float nextPosX;

    //List of possilbe items to spawn
    public List<BackgroundPiece> objects = new List<BackgroundPiece>();

    public void GenerateBackground(Transform tile)
    {
        nextPosX = tile.position.x + Random.Range(minOffset, maxOffset);

        //get item to spawn
        BackgroundPiece levelObject = ChooseObject();

        //Only spawn if not null
        if (levelObject != null && levelObject.prefab != null)
        {
            GameObject item = (GameObject)Instantiate(levelObject.prefab, new Vector3(nextPosX, tile.position.y, levelObject.prefab.transform.position.z), Quaternion.identity);
            item.name = levelObject.prefab.name;

            item.transform.SetParent(tile);
        }
    }

    BackgroundPiece ChooseObject()
    {
        //List of elegible tiles, and sort based on probability
        List<BackgroundPiece> possibleObjects = new List<BackgroundPiece>();

        //If tile is within the generation range, add it to the list
        foreach (BackgroundPiece o in objects)
            if ((transform.position.x >= o.minDistance) && (transform.position.x <= o.maxDistance || o.maxDistance == 0))
                possibleObjects.Add(o);

        //Sort the list by probability (since it is using cumulative probability)
        possibleObjects.Sort((x, y) => x.probability.CompareTo(y.probability));

        //The probability of all tiles added together
        float maxProbability = 0;
        //Running cumulative rpobability
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < possibleObjects.Count; i++)
            maxProbability += possibleObjects[i].probability;

        //Choose tile
        for (int i = 0; i < possibleObjects.Count; i++)
        {
            //Add to running probability
            cumulativeProbability += possibleObjects[i].probability;

            //Generate a random number
            float roll = Random.Range(0, maxProbability);

            //If number is within range
            if (roll < cumulativeProbability)
            {
                return possibleObjects[i];
            }

        }

        return null;
    }
}

[System.Serializable]
public class BackgroundPiece
{
    public GameObject prefab;

    public float probability = 1f;
    public float minDistance = 0f;
    public float maxDistance = 0f;
}
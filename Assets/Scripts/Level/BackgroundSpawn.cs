using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundSpawn : MonoBehaviour
{
    //List of possilbe items to spawn
    public List<BackgroundPiece> objects = new List<BackgroundPiece>();

    void Start()
    {
        //get item to spawn
        BackgroundPiece levelObject = ChooseObject();

        //Only spawn if not null
        if (levelObject != null && levelObject.prefab != null)
        {
            GameObject item = (GameObject)Instantiate(levelObject.prefab, transform.position, Quaternion.identity);
            item.name = levelObject.prefab.name;
            //Item replaces this gameobject in heirarchy, so they should have the same parent
            item.transform.SetParent(transform.parent);
        }

        //Destroy this spawning object, even if no item was spawned
        Destroy(gameObject);
    }

    BackgroundPiece ChooseObject()
    {
        //List of prefabs, to be sorted
        List<BackgroundPiece> possibleObjects = new List<BackgroundPiece>();

        //If item is within the generation range, add it to the list
        foreach (BackgroundPiece i in objects)
            if (transform.position.x >= i.minDistance)
                possibleObjects.Add(i);

        //Sort the list by probability (since it is using cumulative probability)
        possibleObjects.Sort((x, y) => x.probability.CompareTo(y.probability));

        //prefab is null by default
        BackgroundPiece prefab = null;

        //The probability of all prefabs added together
        float maxProbability = 0;
        //Running cumulative rpobability
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < possibleObjects.Count; i++)
            maxProbability += possibleObjects[i].probability;

        //Choose prefab
        for (int i = 0; i < possibleObjects.Count; i++)
        {
            //Add to running probability
            cumulativeProbability += possibleObjects[i].probability;

            //Generate a random number
            float roll = Random.Range(0, maxProbability);

            //If number is within range
            if (roll < cumulativeProbability)
            {
                //Set prefab as this prefab, then break
                prefab = possibleObjects[i];
                break;
            }

        }

        //Return the prefab that was chosen
        return prefab;
    }

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Vector3 size = new Vector3(2, 2, 1);

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(pos, size);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos, size);
    }
}

[System.Serializable]
public class BackgroundPiece
{
    public GameObject prefab;

    public float probability = 1f;
    public float minDistance = 0f;
}
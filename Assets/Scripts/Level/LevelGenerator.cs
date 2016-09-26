using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Tooltip("Should the level be regenerated on game start?")]
    public bool generateOnPlay = false;

    [Header("Level Values")]
    [Tooltip("The length (in metres) of each tile.")]
    public float tileLength = 20f;
    [Tooltip("How many tiles there are in the level.")]
    public int levelLength = 10;

    [Header("Tiles")]
    public GameObject startTile;
    [Space()]
    public LevelTile[] tiles;
    [Space()]
    public GameObject endTile;

    public void Start()
    {
        //Generate the level on start if desired
        if(generateOnPlay)
            Generate();
    }

    public void Generate()
    {
        //Destroy current level objects
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
            children.Add(child);

        foreach (Transform child in children)
            DestroyImmediate(child.gameObject);

        //Generate level
        for(int i = 0; i < levelLength; i++)
        {
            //Starttile is default
            GameObject prefab = startTile;

            //Choose random tile
            if (i > 0)
                prefab = GetRandomTile();
            //If end of level, choose endTile
            if (i == levelLength - 1)
                prefab = endTile;

            //Instantiate tile at correct position
            GameObject tile = (GameObject)Instantiate(prefab, new Vector3(tileLength * i, 0, 0), Quaternion.identity);
            //Parent to this gameobject
            tile.transform.SetParent(transform);
        }
    }

    /// <summary>
    /// Returns a random tile based on the probability of all tiles.
    /// </summary>
    /// <returns></returns>
    GameObject GetRandomTile()
    {
        //Starttile by default (prevents returning null)
        GameObject tile = startTile;

        //The probability of all tiles added together
        float maxProbability = 0;
        //Running cumulative rpobability
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < tiles.Length; i++)
            maxProbability += tiles[i].probability;

        //Choose tile
        for (int i = 0; i < tiles.Length; i++)
        {
            //Add to running probability
            cumulativeProbability += tiles[i].probability;

            //Generate a random number
            float roll = Random.Range(0, maxProbability);

            //If number is within range
            if (roll < cumulativeProbability)
            {
                //Set tile as this tile, then break
                tile = tiles[i].prefab;
                break;
            }
        }

        return tile;
    }
}

[System.Serializable]
public class LevelTile
{
    [Tooltip("The prefab to instantiate for this tile.")]
    public GameObject prefab;

    [Range(0f, 1f)]
    [Tooltip("How likely this tile is to be chosen.")]
    public float probability = 1f;
}
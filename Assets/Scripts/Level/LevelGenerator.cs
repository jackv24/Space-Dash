using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Values")]
    [Tooltip("The length (in metres) of each tile.")]
    public float tileLength = 20f;

    [Space()]
    [Tooltip("List of all tiles which can be generated. Will be sorted in order of probability when level is generated.")]
    public List<LevelTile> tiles = new List<LevelTile>();
    public GameObject startTile;

    [Header("Tracking Player")]
    public Transform player;
    [Tooltip("How many tiles ahead to generate from the player.")]
    public int lengthAhead = 2;
    [Tooltip("How many tiles to generate before old tiles start getting deleted.")]
    public int maxLoadedTiles = 5;
    private float nextGeneratePlayerPos = 0;

    //List of all generated tiles
    private List<GameObject> generatedTiles = new List<GameObject>();
    //The index of the last tile generated
    private int lastTileIndex = 0;

    void Start()
    {
        //Make sure to delete preview before starting
        Reset();
    }

    void Update()
    {
        //If player has reaches next tile threshold
        if (player.position.x >= nextGeneratePlayerPos)
        {
            //Update threshold
            nextGeneratePlayerPos += tileLength;

            //Generate the next tile
            GenerateNextTile();

            //Cull old tiles if needed
            if (generatedTiles.Count > maxLoadedTiles)
            {
                Destroy(generatedTiles[0]);
                generatedTiles.RemoveAt(0);
            }
        }
    }

    public void Reset()
    {
        List<Transform> children = new List<Transform>();
        //Get all children
        foreach (Transform child in transform)
            children.Add(child);

        //Destroy all children
        foreach (Transform child in children)
            DestroyImmediate(child.gameObject);

        //Clear generated tiles list
        generatedTiles.Clear();
        //reset tile pos
        lastTileIndex = 0;

        //Reset threshold
        nextGeneratePlayerPos = tileLength * -lengthAhead;
    }

    public void GeneratePreview(int levelLength)
    {
        Reset();

        //Generate specified number of tiles
        for (int i = 0; i < levelLength; i++)
            GenerateNextTile();
    }

    public void GenerateNextTile()
    {
        //Starttile is default
        GameObject prefab = startTile;

        //Choose random tile
        if (lastTileIndex > 0)
            prefab = GetRandomTile();

        //Instantiate tile at correct position
        GameObject tile = (GameObject)Instantiate(prefab, new Vector3(tileLength * lastTileIndex, 0, 0), Quaternion.identity);
        //Parent to this gameobject
        tile.transform.SetParent(transform);

        generatedTiles.Add(tile);
        lastTileIndex++;
    }

    // Returns a random tile based on the probability of all tiles.
    GameObject GetRandomTile()
    {
        //List of elegible tiles, and sort based on probability
        List<LevelTile> possibleTiles = new List<LevelTile>();

        //If tile is within the generation range, add it to the list
        foreach (LevelTile t in tiles)
            if (lastTileIndex * tileLength >= t.minDistance)
                possibleTiles.Add(t);

        //Sort the list by probability (since it is using cumulative probability)
        possibleTiles.Sort((x, y) => x.probability.CompareTo(y.probability));

        //Starttile by default (prevents returning null)
        GameObject tile = startTile;

        //The probability of all tiles added together
        float maxProbability = 0;
        //Running cumulative rpobability
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < possibleTiles.Count; i++)
            maxProbability += possibleTiles[i].probability;

        //Choose tile
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            //Add to running probability
            cumulativeProbability += possibleTiles[i].probability;

            //Generate a random number
            float roll = Random.Range(0, maxProbability);

            //If number is within range
            if (roll < cumulativeProbability)
            {
                //Set tile as this tile, then break
                tile = possibleTiles[i].prefab;
                break;
            }

        }

        //Return the tile that was chosen
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

    [Tooltip("How far along the level until this tile starts generating (in metres).")]
    public float minDistance = 0f;
}
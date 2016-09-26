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

    private List<GameObject> generatedTiles = new List<GameObject>();
    private int lastTilePos = 0;

    void Start()
    {
        //Sort tiles based on probability (ascending) for cumulative probability
        tiles.Sort((x, y) => x.probability.CompareTo(y.probability));

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
        lastTilePos = 0;

        //Reset threshold
        nextGeneratePlayerPos = tileLength * -lengthAhead;
    }

    public void GeneratePreview(int levelLength)
    {
        Reset();

        //Sort tiles based on probability (ascending) for cumulative probability
        tiles.Sort((x, y) => x.probability.CompareTo(y.probability));

        //Generate specified number of tiles
        for (int i = 0; i < levelLength; i++)
            GenerateNextTile();
    }

    public void GenerateNextTile()
    {
        //Starttile is default
        GameObject prefab = startTile;

        //Choose random tile
        if (lastTilePos > 0)
            prefab = GetRandomTile();

        //Instantiate tile at correct position
        GameObject tile = (GameObject)Instantiate(prefab, new Vector3(tileLength * lastTilePos, 0, 0), Quaternion.identity);
        //Parent to this gameobject
        tile.transform.SetParent(transform);

        generatedTiles.Add(tile);
        lastTilePos++;
    }

    // Returns a random tile based on the probability of all tiles.
    GameObject GetRandomTile()
    {
        //Starttile by default (prevents returning null)
        GameObject tile = startTile;

        //The probability of all tiles added together
        float maxProbability = 0;
        //Running cumulative rpobability
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < tiles.Count; i++)
            maxProbability += tiles[i].probability;

        //Choose tile
        for (int i = 0; i < tiles.Count; i++)
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
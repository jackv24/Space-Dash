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
    public TileGroup startGroup;
    //The group that is currently being generated
    private TileGroup currentGroup;
    //The index in the current group
    private int currentGroupIndex = 0;

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

    private BackgroundSpawn backgroundSpawn;

    void Awake()
    {
        backgroundSpawn = GetComponent<BackgroundSpawn>();
    }

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
        currentGroupIndex = 0;

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
        if (lastTileIndex <= 0 && startGroup)
            currentGroup = startGroup;
        else if (!startGroup || currentGroupIndex >= currentGroup.length)
        {
            //Choose random tile
            currentGroup = GetRandomTile();
            currentGroupIndex = 0;
        }

        GameObject prefab = null;

        //Choose start, end, or random tile
        if (currentGroupIndex == 0 && currentGroup.startTile)
            prefab = currentGroup.startTile;
        else if (currentGroupIndex >= currentGroup.length - 1 && currentGroup.endTile)
            prefab = currentGroup.endTile;
        else
            prefab = currentGroup.GetRandomTile();

        //Instantiate tile at correct position
        GameObject tile = (GameObject)Instantiate(prefab, new Vector3(tileLength * lastTileIndex, prefab.transform.position.y + transform.position.y, 0), Quaternion.identity);
        //Parent to this gameobject
        tile.transform.SetParent(transform);

        generatedTiles.Add(tile);
        lastTileIndex++;
        currentGroupIndex++;

        if (!backgroundSpawn)
            backgroundSpawn = GetComponent<BackgroundSpawn>();
        if(backgroundSpawn)
            backgroundSpawn.GenerateBackground(tile.transform);
        
    }

    // Returns a random tile based on the probability of all tiles.
    TileGroup GetRandomTile()
    {
        //List of elegible tiles, and sort based on probability
        List<LevelTile> possibleTiles = new List<LevelTile>();

        //If tile is within the generation range, add it to the list
        foreach (LevelTile t in tiles)
            if ((lastTileIndex * tileLength >= t.minDistance) && (lastTileIndex * tileLength <= t.maxDistance || t.maxDistance == 0))
                possibleTiles.Add(t);

        //Sort the list by probability (since it is using cumulative probability)
        possibleTiles.Sort((x, y) => x.probability.CompareTo(y.probability));

        //Starttile by default (prevents returning null)
        TileGroup tile = startGroup;

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
                if (possibleTiles[i].group)
                    tile = possibleTiles[i].group;
                else
                {
                    //If it is a single tile, create a group with only that tile
                    tile = (TileGroup)ScriptableObject.CreateInstance(typeof(TileGroup));
                    tile.tiles.Add(new Tile(possibleTiles[i].prefab));
                    tile.length = 1;
                }
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
    [Tooltip("Can be assigned if a group of tiles is to be generated. If this is assigned, all options below mean nothing.")]
    public TileGroup group;
    [Tooltip("The prefab to instantiate for this tile.")]
    public GameObject prefab;

    [Space()]
    [Range(0f, 1f)]
    [Tooltip("How likely this tile is to be chosen.")]
    public float probability = 1f;

    [Tooltip("How far along the level until this tile starts generating (in metres).")]
    public float minDistance = 0f;
    [Tooltip("When to stop generating tiles (leave at 0 if they should never stop).")]
    public float maxDistance = 0f;
}
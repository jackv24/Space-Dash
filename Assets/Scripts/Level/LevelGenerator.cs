﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

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

    [Space()]
    public GameObject distanceMonolith;
    private float bestPlayerDistance;
    private bool hasGeneratedMonolith = false;

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
        instance = this;

        backgroundSpawn = GetComponent<BackgroundSpawn>();
    }

    void Start()
    {
        if (!player)
            player = GameObject.FindWithTag("Player").transform;

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

        //Calculate what tile the player is on, if in editor
        if (DebugInfo.displayDebugInfo)
        {
            //Get playr pos and add half a tile length (otherwise it would only go to next tile in the middle of it)
            float playerPos = player.position.x + (tileLength / 2);

            //Get closest index to this tile, in number of tiles
            int index = (int)Mathf.Floor(playerPos/ tileLength);
            //Convert from number of tiles to index of currently loaded tile
            int offset = lastTileIndex - maxLoadedTiles;

            //Dont offset of not needed (max loaded tiles is not yet reached)
            if (offset < 0)
                offset = 0;

            //Adjust index for this offset
            index -= offset;

            //If there is a tile in this index, set name in DebugInfo
            if (generatedTiles[index])
                DebugInfo.currentTile = generatedTiles[index].name;
            else
                DebugInfo.currentTile = "Invalid";
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

        foreach (LevelTile t in tiles)
            t.nextSpawnPos = 0;

        //reset tile pos
        lastTileIndex = 0;
        currentGroupIndex = 0;

        //Reset threshold
        nextGeneratePlayerPos = tileLength * -lengthAhead;

        bestPlayerDistance = PlayerPrefs.GetFloat("BestDistance", 0);
        hasGeneratedMonolith = false;
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
        tile.name = prefab.name;

        generatedTiles.Add(tile);
        lastTileIndex++;
        currentGroupIndex++;

        if (!backgroundSpawn)
            backgroundSpawn = GetComponent<BackgroundSpawn>();
        if(backgroundSpawn)
            backgroundSpawn.GenerateBackground(tile.transform);

        //Generate distance monolith, but only once
        if (distanceMonolith && !hasGeneratedMonolith && lastTileIndex * tileLength >= bestPlayerDistance && bestPlayerDistance > 0)
        {
            hasGeneratedMonolith = true;

            //Parent monolith to most recent tile so that it is deleted correctly
            Instantiate(distanceMonolith, tile.transform);
        }
    }

    // Returns a random tile based on the probability of all tiles.
    TileGroup GetRandomTile()
    {
        //List of elegible tiles, and sort based on probability
        List<LevelTile> possibleTiles = new List<LevelTile>();

        //If tile is within the generation range, add it to the list
        foreach (LevelTile t in tiles)
            if ((lastTileIndex * tileLength >= t.minDistance) && (lastTileIndex * tileLength <= t.maxDistance || t.maxDistance == 0) && lastTileIndex >= t.nextSpawnPos)
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

                possibleTiles[i].nextSpawnPos = lastTileIndex + possibleTiles[i].spacing + 1;

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
    [Tooltip("How likely this tile is to be chosen.")]
    public float probability = 1f;

    [Tooltip("How far along the level until this tile starts generating (in metres).")]
    public float minDistance = 0f;
    [Tooltip("When to stop generating tiles (leave at 0 if they should never stop).")]
    public float maxDistance = 0f;

    [Space()]
    [Tooltip("How far after spawning this tile until another can be spawned (in number of tiles).")]
    public int spacing = 0;
    //The next index position at which this tile can spawn
    [HideInInspector]
    public int nextSpawnPos = 0;
}
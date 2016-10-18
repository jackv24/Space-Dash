using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileGroup", menuName = "Tile Group")]
public class TileGroup : ScriptableObject
{
    public GameObject startTile = null;

    [Tooltip("The tiles to generate in this group.")]
    public List<Tile> tiles = new List<Tile>();

    public GameObject endTile = null;

    [Tooltip("How many tiles to generate, if not in order.")]
    public int length = 5;

    public GameObject GetRandomTile()
    {        //Copy tile list into new list for sorting
        List<Tile> possibleTiles = new List<Tile>(tiles);

        possibleTiles.Sort((x, y) => x.probability.CompareTo(y.probability));

        Tile tile = null;

        float maxProbability = 0;
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < possibleTiles.Count; i++)
            maxProbability += possibleTiles[i].probability;

        //Choose from available tiles
        for (int i = 0; i < possibleTiles.Count; i++)
        {
            //Add cumulative probability
            cumulativeProbability += possibleTiles[i].probability;

            //Get random number in range
            float roll = Random.Range(0, maxProbability);

            //If this number is within the cumulative probability
            if (roll < cumulativeProbability)
            {
                //Choose this tile
                tile = possibleTiles[i];
                break;
            }

        }

        return tile.prefab;
    }
}

[System.Serializable]
public class Tile
{
    public Tile(GameObject prefab)
    {
        this.prefab = prefab;
    }

    [Tooltip("The prfab to instantiate for the tile.")]
    public GameObject prefab;

    [Tooltip("How likely this tile is to be chosen (if Generate in Order is set to false, otherwise it is ignored).")]
    public float probability = 1f;
}
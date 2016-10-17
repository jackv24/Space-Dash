using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileGroup", menuName = "Tile Group")]
public class TileGroup : ScriptableObject
{
    [Tooltip("The tiles to generate in this group.")]
    public List<Tile> tiles = new List<Tile>();

    [Space()]
    [Tooltip("Should these tiles be generated in order? If not, tiles are generated based on probability.")]
    public bool generateInOrder = true;

    public GameObject GetRandomTile()
    {
        //Copy tile list into new list for sorting
        List<Tile> possibleTiles = new List<Tile>(tiles);

        possibleTiles.Sort((x, y) => x.probability.CompareTo(y.probability));

        Tile tile = null;

        float maxProbability = 0;
        float cumulativeProbability = 0;

        for (int i = 0; i < possibleTiles.Count; i++)
            maxProbability += possibleTiles[i].probability;

        for (int i = 0; i < possibleTiles.Count; i++)
        {
            cumulativeProbability += possibleTiles[i].probability;

            float roll = Random.Range(0, maxProbability);

            if (roll < cumulativeProbability)
            {
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

    [Tooltip("How likely this tile is to be chosen (if Generate in Order is set to true, otherwise it is ignored).")]
    public float probability = 1f;
}
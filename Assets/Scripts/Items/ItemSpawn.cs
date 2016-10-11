using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawn : MonoBehaviour
{
    //List of possilbe items to spawn
    public List<LevelItem> items = new List<LevelItem>();

    void Start()
    {
        //get item to spawn
        GameObject itemPrefab = ChooseItem();

        //Only spawn if not null
        if (itemPrefab != null)
        {
            GameObject item = (GameObject)Instantiate(itemPrefab, transform.position, Quaternion.identity);
            item.name = itemPrefab.name;
            //Item replaces this gameobject in heirarchy, so they should have the same parent
            item.transform.SetParent(transform.parent);
        }

        //Destroy this spawning object, even if no item was spawned
        Destroy(gameObject);
    }
    
    GameObject ChooseItem()
    {
        //List of items, to be sorted
        List<LevelItem> possibleItems = new List<LevelItem>();

        //If item is within the generation range, add it to the list
        foreach (LevelItem i in items)
            if (transform.position.x >= i.minDistance)
                possibleItems.Add(i);

        //Sort the list by probability (since it is using cumulative probability)
        possibleItems.Sort((x, y) => x.probability.CompareTo(y.probability));

        //Item is null by default
        GameObject item = null;

        //The probability of all items added together
        float maxProbability = 0;
        //Running cumulative rpobability
        float cumulativeProbability = 0;

        //Get max probability
        for (int i = 0; i < possibleItems.Count; i++)
            maxProbability += possibleItems[i].probability;

        //Choose item
        for (int i = 0; i < possibleItems.Count; i++)
        {
            //Add to running probability
            cumulativeProbability += possibleItems[i].probability;

            //Generate a random number
            float roll = Random.Range(0, maxProbability);

            //If number is within range
            if (roll < cumulativeProbability)
            {
                //Set item as this item, then break
                item = possibleItems[i].prefab;
                break;
            }

        }

        //Return the item that was chosen
        return item;
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
public class LevelItem
{
    public GameObject prefab;

    public float probability = 1f;
    public float minDistance = 0f;
}

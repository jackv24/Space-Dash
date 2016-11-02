using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawn : MonoBehaviour
{
    //List of possible items to spawn
    public List<LevelItem> items = new List<LevelItem>();

    private static List<LevelItem> cantSpawnItems = new List<LevelItem>();

    void Start()
    {
        Spawn(false);

        //Destroy this spawning object, even if no item was spawned
        Destroy(gameObject);
    }
    
    LevelItem ChooseItem()
    {
        //List of items, to be sorted
        List<LevelItem> possibleItems = new List<LevelItem>();

        //If item is within the generation range, add it to the list
        foreach (LevelItem i in items)
            if (transform.position.x >= i.minDistance)
            {
                bool canAdd = true;

                //Loop through items that cant be spawned
                foreach (LevelItem j in cantSpawnItems)
                {
                    //If this item is in the list, and past the distance it can be spawned
                    if (j.prefab == i.prefab)
                    {
                        if (transform.position.x > j.nextSpawnPos)
                        {
                            possibleItems.Add(i);
                            cantSpawnItems.Remove(j);
                        }

                        canAdd = false;
                    }
                }

                if (canAdd)
                    possibleItems.Add(i);
            }

        //Sort the list by probability (since it is using cumulative probability)
        possibleItems.Sort((x, y) => x.probability.CompareTo(y.probability));

        //Item is null by default
        LevelItem item = null;

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
                item = possibleItems[i];
                break;
            }

        }

        //Return the item that was chosen
        return item;
    }

    public void Spawn(bool parent)
    {
        //Delete preview preview object
        while(transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        //get item to spawn
        LevelItem levelItem = ChooseItem();

        //Only spawn if not null
        if (levelItem != null && levelItem.prefab != null)
        {
            float distance = 0;

            for (int i = 0; i < levelItem.chainLength; i++)
            {
                Vector3 offset = Vector3.zero;
                float curvePos = (levelItem.chainLength > 1) ? levelItem.chainCurve.Evaluate(i / (float)(levelItem.chainLength - 1)) : 0;

                switch (levelItem.chainDirection)
                {
                    case LevelItem.Direction.Up:
                        offset = new Vector3(curvePos, distance, 0);
                        break;
                    case LevelItem.Direction.Down:
                        offset = new Vector3(curvePos, -distance, 0);
                        break;
                    case LevelItem.Direction.Left:
                        offset = new Vector3(-distance, curvePos, 0);
                        break;
                    case LevelItem.Direction.Right:
                        offset = new Vector3(distance, curvePos, 0);
                        break;
                }

                GameObject item = (GameObject)Instantiate(levelItem.prefab, transform.position + offset, Quaternion.identity);
                item.name = levelItem.prefab.name;
                //Item replaces this gameobject in heirarchy, so they should have the same parent
                item.transform.SetParent(parent ? transform : transform.parent);

                distance += levelItem.chainSpacing;
            }

            //Prevent items from spawnng for some time, if needed
            if (levelItem.spacing > 0)
            {
                levelItem.nextSpawnPos = transform.position.x + levelItem.spacing;
                cantSpawnItems.Add(levelItem);
            }
        }
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
    [Tooltip("The game object that will be instantiated.")]
    public GameObject prefab;

    [Tooltip("How likely it is that this item will be generated (try and make all items probability add up to 1, otherwise it is hard to visualise).")]
    public float probability = 1f;
    [Tooltip("This item will not start generating until this distance has been reached.")]
    public float minDistance = 0f;

    [Space()]
    [Tooltip("How many metres until this item can be spawned again after spawning.")]
    public float spacing = 0f;
    //When this can next be spawned
    [HideInInspector]
    public float nextSpawnPos = 0f;

    [Space()]
    [Tooltip("The minimum amount of this item that will be generated in a chain.")]
    public int chainLength = 1;

    [Tooltip("How much space between items in the chain.")]
    public float chainSpacing = 2f;

    public enum Direction { Left, Right, Up, Down }
    [Space()]
    [Tooltip("The direction that the chain will be generated in (will have no effect if the chain length is one, of course).")]
    public Direction chainDirection = Direction.Right;
    [Tooltip("How the spawned items will be offset along their length.")]
    public AnimationCurve chainCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
}

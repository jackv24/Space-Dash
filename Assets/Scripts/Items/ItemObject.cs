using UnityEngine;
using System.Collections;

public class ItemObject : MonoBehaviour
{
    //The itemdata object that this gameobject represents
    public ItemData itemData;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        //If this item has data, and the other collider has an inventory
        if (inventory && itemData)
        {
            //Add this item to the inventory
            inventory.items.Add(itemData);

            //Destroy gameobject in world
            Destroy(gameObject);
        }
    }
}

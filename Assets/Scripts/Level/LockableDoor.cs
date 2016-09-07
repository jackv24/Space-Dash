using UnityEngine;
using System.Collections;

public class LockableDoor : MonoBehaviour
{
    [Tooltip("The item that the player must have to open this door. If left empty, the door will open unconditionally.")]
    public ItemData requiredItem;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        //If the collider that entered has an inventory
        if (inventory)
        {
            //If this door requires an item...
            if (requiredItem)
            {
                //...only open if inventory has item
                if (inventory.items.Contains(requiredItem))
                    gameObject.SetActive(false);
            }
            else
            {
                //If door does not require item, open anyway
                gameObject.SetActive(false);
            }
        }
    }
}

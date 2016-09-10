using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Display")]
    [Tooltip("The UI Text object to display inventory contents.")]
    public Text inventoryText;
    private string inventoryString;

    [Tooltip("The string to format item names with (must contain '{0}').")]
    public string itemDisplayString = " - {0}";

    //InventoryItem as a class to hold additional inventory-related data that should not be on the item itself
    [System.Serializable]
    public class InventoryItem
    {
        //InventoryItems must be created with ItemData
        public InventoryItem(ItemData item)
        {
            data = item;
        }

        public ItemData data;
        public int amount = 1;
    }

    [Header("Data")]
    [Tooltip("A list of all items in the inventory. This should not be edited manually (unless it is desired to start the player with some items).")]
    public List<InventoryItem> items = new List<InventoryItem>();

    [Space()]
    [Tooltip("Should the inventory be sorted alphabetically?")]
    public bool isSorted = true;

    void Start()
    {
        if (inventoryText)
        {
            //Cache initial string for formatting
            inventoryString = inventoryText.text;

            //Remove formatting characters
            inventoryText.text = string.Format(inventoryString, "");
        }
    }

    /// <summary>
    /// Adds an item to the inventory, stacking if necessary.
    /// </summary>
    /// <param name="data">The item to add to the inventory.</param>
    public void Add(ItemData data)
    {
        //For checking if the item exists
        bool alreadyExists = false;

        //Loop through all existing items
        foreach (var item in items)
        {
            //Ff item is in inventory already
            if (item.data == data)
            {
                alreadyExists = true;
                //Increase the amount
                item.amount++;
            }
        }

        //If it doesn't exist, add it as a new InventoryItem
        if(!alreadyExists)
            items.Add(new InventoryItem(data));

        //Sort the inventory list alphabetically if desired
        if (isSorted)
            items.Sort((x, y) => x.data.displayName.CompareTo(y.data.displayName));

        //Finally, update the display
        UpdateDisplay();
    }

    /// <summary>
    /// Removes a specified amount of an item from the inventory.
    /// </summary>
    /// <param name="data">The item to remove.</param>
    /// <param name="amount">The amount to remove.</param>
    public void Remove(ItemData data, int amount)
    {
        //The item to remove from the inventory
        InventoryItem removeItem = null;

        //Loop through inventory
        foreach (var item in items)
        {
            //If item is found
            if (item.data == data)
            {
                //Decrement amount
                item.amount -= amount;

                //If amount is less than or equal to zero, it should be removed
                if (item.amount <= 0)
                    removeItem = item;
            }
        }

        //If an item should be removed, remove it
        if (removeItem != null)
            items.Remove(removeItem);

        //Finally, update the display
        UpdateDisplay();
    }

    /// <summary>
    /// Checks if the inventory contains a specified amount of an item.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <param name="amount">The amount needed.</param>
    /// <returns></returns>
    public bool HasItem(ItemData data, int amount)
    {
        //Iterate over inventory
        foreach (var item in items)
            //If item exists and has required amount
            if (item.data == data && item.amount >= amount)
                return true;

        return false;
    }

    /// <summary>
    /// Updates the inventory display.
    /// </summary>
    public void UpdateDisplay()
    {
        //If the text object is assigned
        if (inventoryText)
        {
            //Text to plug into inventoryText
            string itemsText = "";

            //Loop through all items in inventory
            foreach (var item in items)
                //Add formatted string to working string itemsText
                itemsText += string.Format(
                    itemDisplayString,
                    //Format item name correctly
                    item.data.displayName + ((item.amount > 1) ? " x" + item.amount : "") + "\n");

            //Finally, plug itemsText into inventory text
            inventoryText.text = string.Format(inventoryString, itemsText);
        }
    }
}

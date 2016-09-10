using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "NewItem", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    //The name to display for this item
    [Header("Item Properties")]
    public string displayName = "ITEM";
}

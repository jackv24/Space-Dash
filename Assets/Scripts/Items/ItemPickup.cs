using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    public enum Type
    {
        Health,
        Oxygen
    }
    [Tooltip("The type of value to affect.")]
    public Type type;

    [Tooltip("How much of the specified type to gain.")]
    public int value = 0;

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerStats stats = col.gameObject.GetComponent<PlayerStats>();

        //If stats was gotten
        if (stats)
        {
            //Add stat based on type
            switch (type)
            {
                case Type.Health:
                    stats.AddHealth(value);
                    break;
                case Type.Oxygen:
                    stats.AddOxygen(value);
                    break;
            }

            //Destroy gameobject so it has been "picked up"
            Destroy(gameObject);
        }
    }
}

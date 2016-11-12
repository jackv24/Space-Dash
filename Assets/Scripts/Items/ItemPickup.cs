using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour
{
    public enum Type
    {
        Health,
        Oxygen,
        ExtraJump,
        ExtraOxygen
    }
    [Tooltip("The type of value to affect.")]
    public Type type;

    public int value = 0;

    public bool reset = false;

    public int pointsValue = 10;
    public Color pickupTextColor = Color.white;
    public float pickupTextScale = 1f;

    public GameObject pickupIconPrefab;

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerStats stats = col.gameObject.GetComponent<PlayerStats>();
        PlayerControl control = col.gameObject.GetComponent<PlayerControl>();

        //If stats was gotten
        if (stats && control)
        {
            //Add stat based on type
            switch (type)
            {
                case Type.Health:
                    stats.AddHealth(value);
                    break;
                case Type.Oxygen:
                    stats.AddOxygen(value);
                    if (SoundManager.instance)
                        SoundManager.instance.PlaySound(SoundManager.instance.sounds.pickupOxygen);
                    HUDControl.instance.ShowOxygenText(value);
                    break;
                case Type.ExtraJump:
                    control.AddJump(reset);
                    value = 1;
                    if (SoundManager.instance)
                        SoundManager.instance.PlaySound(SoundManager.instance.sounds.pickupJumpPowerup);
                    break;
                case Type.ExtraOxygen:
                    stats.IncreaseOxygen(value, reset);
                    if (SoundManager.instance)
                        SoundManager.instance.PlaySound(SoundManager.instance.sounds.pickupOxygenPowerup);
                    break;
            }

            stats.AddScore(pointsValue);

            HUDControl.instance.ShowPickupText(pointsValue, transform.position, pickupTextColor, pickupTextScale);

            if (pickupIconPrefab)
                HUDControl.instance.ShowPickupIcon(pickupIconPrefab);

            //Destroy gameobject so it has been "picked up"
            Destroy(gameObject);
        }
    }
}

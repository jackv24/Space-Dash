﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    public enum Type
    {
        Health,
        Oxygen,
        ExtraJump,
        ExtraOxygen,
        Nothing
    }
    [Tooltip("The type of value to affect.")]
    public Type type;

    public int value = 0;

    [Tooltip("How many metres until this item can be spawned again after spawning.")]
    public float spacing = 0f;

    public bool reset = false;

    public int pointsValue = 10;
    public Color pickupTextColor = Color.white;
    public float pickupTextScale = 1f;
    public bool showInHud = true;

    private List<GameObject> chainObjects = new List<GameObject>();
    private int chainBonus = 0;
    public Color chainScoreColor = Color.white;
    public float pickupBonusScale = 1.5f;

    public GameObject pickupIconPrefab;
    public GameObject pickupParticles;

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerCharStats stats = col.gameObject.GetComponent<PlayerCharStats>();
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

            HUDControl.instance.ShowPickupText(pointsValue, transform.position, pickupTextColor, pickupTextScale, !showInHud);

            //Award score if entire chain was picked up
            if (chainObjects.Count >= GameManager.instance.minPickupChain - 1)
            {
                int collectedItems = 0;

                foreach (var item in chainObjects)
                {
                    if (item == null)
                        collectedItems++;
                }

                if (collectedItems == chainObjects.Count)
                {
                    stats.AddScore(chainBonus);

                    if (SoundManager.instance)
                        SoundManager.instance.PlaySound(SoundManager.instance.sounds.chainBonus);

                    HUDControl.instance.ShowPickupText(chainBonus, transform.position, chainScoreColor, pickupBonusScale, true);
                }
            }

            if (pickupIconPrefab)
                HUDControl.instance.ShowPickupIcon(pickupIconPrefab);

            if (pickupParticles)
                Instantiate(pickupParticles, transform.position, pickupParticles.transform.rotation);

            //Destroy gameobject so it has been "picked up"
            Destroy(gameObject);
        }
    }

    public void SetChainEnd(List<GameObject> chain)
    {
        chainObjects = chain;

        if(GameManager.instance)
            chainBonus = Mathf.CeilToInt(pointsValue * (chain.Count + 1) * GameManager.instance.pickupChainBonus);
    }
}

using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    //Health variables
    public int currentHealth = 100;
    public int maxHealth = 100;

    //How long it takes to respawn after dying
    public float respawnTime = 1f;

    private Vector3 initialPosition;

    void Start()
    {
        //Cache initial position
        initialPosition = transform.position;
    }

    public void RemoveHealth(int amount)
    {
        currentHealth -= amount;

        //When health falls to or below zero
        if (currentHealth <= 0)
        {
            //Clamp it at zero
            currentHealth = 0;

            Die();
        }
    }

    public void Die()
    {
        //Play death transition on main camera
        Camera.main.SendMessage("PlayTransition");

        GameManager.instance.EndRun();

        //Start respawn countdown
        StartCoroutine("Respawn", respawnTime);
    }

    IEnumerator Respawn(float time)
    {
        yield return new WaitForSeconds(time);

        //Reset level if there is a level generator
        LevelGenerator generator = FindObjectOfType<LevelGenerator>();
        if (generator)
            generator.Reset();

        //Reset position
        transform.position = initialPosition;

        //Reset health
        currentHealth = maxHealth;
    }
}

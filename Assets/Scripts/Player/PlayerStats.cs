using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    private int score;
    public int Score { get { return score + (int)transform.position.x; } }

    [Header("Health")]
    public int currentHealth = 100;
    public int maxHealth = 100;

    [Space()]
    [Tooltip("How many second after dying until the player respawns.")]
    public float respawnTime = 1f;

    [Header("Oxygen")]
    public int currentOxygen = 100;
    public int maxOxygen = 100;

    [Space()]
    [Tooltip("How much oxygen should be depleted per second.")]
    public int depletionRate = 1;

    //Returns true if health and oxygen are above zero, false otherwise
    public bool IsAlive { get { return (currentHealth > 0 && currentOxygen > 0) ? true : false; } }
    private bool hasAlreadyDied = false;

    private Vector3 initialPosition;

    void Start()
    {
        //Cache initial position
        initialPosition = transform.position;

        StartCoroutine("DepleteOxygen");
    }


    public void AddHealth(int amount)
    {
        //Remove specified amount of health
        currentHealth += amount;

        //Clamp
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void RemoveHealth(int amount)
    {
        //Remove specified amount of health
        currentHealth -= amount;

        //When health falls to or below zero
        if (currentHealth <= 0)
        {
            //Clamp it at zero
            currentHealth = 0;

            Die();
        }
    }

    public void AddOxygen(int amount)
    {
        //Remove specified amount of health
        currentOxygen += amount;

        //Clamp
        if (currentOxygen > maxOxygen)
            currentOxygen = maxOxygen;
    }

    public void RemoveOxygen(int amount)
    {
        //Remove specified amount of oxygen
        currentOxygen -= amount;

        //When oxygen falls to or below zero
        if (currentOxygen <= 0)
        {
            //Clamp it at zero
            currentOxygen = 0;

            Die();
        }
    }

    public void AddScore(int amount)
    {
        //Remove specified amount of health
        score += amount;
    }

    public void Die()
    {
        //Ensure the player does not die more than once before respawning
        if (!hasAlreadyDied)
        {
            hasAlreadyDied = true;

            //Play death transition on main camera
            Camera.main.SendMessage("PlayTransition");

            //Start respawn countdown
            StartCoroutine("Respawn", respawnTime);

            //Save data
            float bestDistance = PlayerPrefs.GetFloat("BestDistance");

            if (transform.position.x > bestDistance)
                PlayerPrefs.SetFloat("BestDistance", transform.position.x);

            //Save high score
            if (Score > PlayerPrefs.GetInt("BestScore"))
                PlayerPrefs.SetInt("BestScore", Score);
        }
    }

    IEnumerator DepleteOxygen()
    {
        while (currentOxygen > 0)
        {
            yield return new WaitForSeconds(1/(float)depletionRate);

            RemoveOxygen(1);
        }
    }

    //Wait's the specified amount of time and then resets game values
    IEnumerator Respawn(float time)
    {
        yield return new WaitForSeconds(time);

        //Reset level if there is a level generator
        LevelGenerator generator = FindObjectOfType<LevelGenerator>();
        if (generator)
            generator.Reset();

        //Reset position
        transform.position = initialPosition;

        //Reset stats
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        score = 0;

        //Start oxygen depletion (should have stopped when the player's oxygen reached 0)
        StopCoroutine("DepleteOxygen");
        StartCoroutine("DepleteOxygen");
    }
}

using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public int currentHealth = 100;
    public int maxHealth = 100;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void RemoveHealth(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        transform.position = initialPosition;

    }
}

using UnityEngine;
using System.Collections;

public class DamageArea : MonoBehaviour
{
    public int damage = 100;

    public enum DamageType { OnEnter, PerSecond }
    public DamageType damageType = DamageType.OnEnter;

    public string targetTag = "Player";

    private PlayerStats stats = null;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            if (stats != other.GetComponent<PlayerStats>())
            {
                stats = other.GetComponent<PlayerStats>();

                switch (damageType)
                {
                    case DamageType.OnEnter:
                        stats.RemoveHealth(damage);
                        break;
                    case DamageType.PerSecond:
                        StartCoroutine("DrainHealth");
                        break;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == targetTag)
            stats = null;
    }

    IEnumerator DrainHealth()
    {
        while (stats)
        {
            stats.RemoveHealth(damage);

            if (stats.currentHealth <= 0)
                break;

            yield return new WaitForSeconds(1f);
        }
    }
}

using UnityEngine;
using System.Collections;

public class SnapperBehaviour : MonoBehaviour
{
    public Transform player;

    [Space()]
    public float anticipationRadius = 8f;
    public float openRadius = 5f;

    private bool hasAnticipated = false;
    private bool hasOpened = false;

    [Space()]
    public Animator anim;

    void Start()
    {
        if (!player)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (anim)
        {
            if (Vector3.Distance(transform.position, player.position) < anticipationRadius && !hasAnticipated)
            {
                anim.SetTrigger("anticipation");
                hasAnticipated = true;
            }

            if (Vector3.Distance(transform.position, player.position) < openRadius && !hasOpened)
            {
                anim.SetBool("isOpen", true);
                hasOpened = true;
            }

            if(player.position.x > transform.position.x)
            {
                anim.SetBool("isOpen", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == player && anim)
        {
            anim.SetBool("isOpen", false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, anticipationRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, openRadius);
    }
}

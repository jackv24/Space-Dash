using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    //Offset from position to move to and from
    [Tooltip("The linear path on which the platform moves.")]
    public Vector2 offset = new Vector2(2, 1);

    //Position in world of offset
    private Vector2 EndPosition { get { return offset + startPos; } }
    //Where the platofrm started
    private Vector2 startPos;
    private Vector2 targetPos;

    [Space()]
    [Tooltip("How fast the platform moves.")]
    public float moveSpeed = 2f;
    [Tooltip("How long to wait once the platform has reached the end of the path, before moving back.")]
    public float waitTime = 2f;

    private float travelTime;

    private Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //Cache start position
        startPos = transform.position;

        travelTime = offset.magnitude / moveSpeed;

        StartCoroutine("Switch");
    }

    void FixedUpdate()
    {
        //Move towards target
        body.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime));
    }

    IEnumerator Switch()
    {
        //Makes it move back and forth
        while (true)
        {
            targetPos = EndPosition;
            yield return new WaitForSeconds(travelTime + waitTime);

            targetPos = startPos;
            yield return new WaitForSeconds(travelTime + waitTime);
        }
    }

    void OnDrawGizmos()
    {
        Vector2 start = (startPos == Vector2.zero) ? (Vector2)transform.position : startPos;

        //Draw yellow line to represent movement path
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, start + offset);
    }
}

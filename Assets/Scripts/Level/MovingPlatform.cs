using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    //Offset from position to move to and from
    [Tooltip("The linear path on which the platform moves.")]
    public Vector2 offset;

    //Position in world of offset
    private Vector2 EndPosition { get { return offset + startPos; } }
    //Where the platofrm started
    private Vector2 startPos;

    [Space()]
    [Tooltip("How fast the platform moves.")]
    public float moveSpeed = 1f;
    [Tooltip("How long to wait once the platform has reached the end of the path, before moving back.")]
    public float waitTime = 2f;

    void Start()
    {
        //Cache start position
        startPos = transform.position;

        //Start movement
        StartCoroutine("Move", waitTime);
    }

    IEnumerator Move(float delay)
    {
        //Where to move to
        Vector3 target = EndPosition;

        while (true)
        {
            yield return new WaitForSeconds(delay);

            while (transform.position != target)
            {
                //Move towards target over time
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            //Switch target between startPos and end pos
            target = ((Vector2)transform.position == EndPosition) ? startPos : EndPosition;
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

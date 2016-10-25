using UnityEngine;
using System.Collections;

public class LinearMove : MonoBehaviour
{
    [Tooltip("The direction to move.")]
    public Vector3 direction = Vector3.right;

    void Update()
    {
        //Move over time
        transform.position += direction * Time.deltaTime;
    }
}

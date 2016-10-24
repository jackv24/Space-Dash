using UnityEngine;
using System.Collections;

public class LinearMove : MonoBehaviour
{
    public Vector3 direction = Vector3.right;

    public float speed = 5f;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}

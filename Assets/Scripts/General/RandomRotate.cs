using UnityEngine;
using System.Collections;

public class RandomRotate : MonoBehaviour
{
    public float rotateSpeedMin = 10f;
    public float rotateSpeedMax = 20f;

    public bool rotateX, rotateY, rotateZ;

    public bool randomStartRotation = false;

    private Vector3 rotationDelta;

    void Start()
    {
        //Get random rotation direction
        if (rotateX)
            rotationDelta.x = Random.Range(rotateSpeedMin, rotateSpeedMax);
        if (rotateY)
            rotationDelta.y = Random.Range(rotateSpeedMin, rotateSpeedMax);
        if (rotateZ)
            rotationDelta.z = Random.Range(rotateSpeedMin, rotateSpeedMax);

        if (randomStartRotation)
            transform.eulerAngles = new Vector3(rotateX ? Random.Range(0, 360) : 0, rotateY ? Random.Range(0, 360) : 0, rotateZ ? Random.Range(0, 360) : 0);
    }

    void Update()
    {
        //Rotate in the chosen direction over time
        transform.Rotate(rotationDelta * Time.deltaTime);
    }
}

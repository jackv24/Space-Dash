﻿using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    //The target to follow
    [Tooltip("The target to follow.\nWill find a GameObject tagged 'Player' if left blank")]
    public Transform target;

    //ow much to offset eh camera from the target
    [Space()]
    [Tooltip("How much to offset camera from the target.")]
    public Vector2 offset;

    //For lerping between positions
    [Range(0f, 1f)]
    [Tooltip("How closely the camera follows the target.\n1 follows perfectly, less than one is smooth.")]
    public float smoothing = 0.5f;

    //Desired camera position (for lerping)
    private Vector3 targetPosition;

    void Start()
    {
        //If there is no target, attempt to find a player's transform
        if (!target)
            target = GameObject.FindWithTag("Player").transform;

        if (target)
        {
            //Start camera at target position
            targetPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            transform.position = targetPosition;
        }
        else
            Debug.Log("Camera has no target!");
    }

    void LateUpdate()
    {
        //Make sure there is a target to follow
        if (target)
        {
            //Set position with offset
            targetPosition.x = target.position.x + offset.x;
            targetPosition.y = target.position.y + offset.y;

            //Lerp camera position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}

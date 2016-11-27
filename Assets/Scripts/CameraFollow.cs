using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    //The target to follow
    [Tooltip("The target to follow.\nWill find a GameObject tagged 'Player' if left blank")]
    public Transform target;

    //ow much to offset eh camera from the target
    [Space()]
    [Tooltip("How much to offset camera from the target.")]
    public Vector2 offset;

    //For lerping between positions
    [Space()]
    [Tooltip("How quickly the camera follows the target.")]
    public float smoothing = 0.5f;

    [Header("Follow Ahead")]
    public bool followAhead = true;
    public float distanceAhead = 5f;
    public float pullAheadSpeed = 10f;
    private float targetDistance;

    //Desired camera position (for lerping)
    private Vector3 targetPosition;

    //Set by player script
    [HideInInspector]
    public Vector2 velocity;

    [Header("Zoom Out")]
    public bool zoomOut = false;
    public LayerMask groundLayer;
    public float maxZoomDistance = -40f;
    [Tooltip("How much padding there should be for detecting if ground is visible.")]
    [Range(0, 1f)]
    public float padding;
    [Tooltip("How much the camera should step out each frame.")]
    public float outStep = 0.1f;
    [Tooltip("How smoothly the camera should return to it's normal distance.")]
    public float smoothIn = 0.1f;
    private float initialDistance;

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

            PlayerCharStats stats = target.GetComponent<PlayerCharStats>();
            if (stats)
                stats.OnReset += Reposition;
        }
        else
            Debug.Log("Camera has no target!");

        initialDistance = transform.position.z;
    }

    void LateUpdate()
    {
        //Make sure there is a target to follow
        if (target && (!GameManager.instance || !GameManager.instance.IsGamePaused))
        {
            //Set position with offset
            targetPosition.x = target.position.x + offset.x;
            targetPosition.y = target.position.y + offset.y;

            if (followAhead)
            {
                //Follow ahead of target based on X input
                targetDistance = Mathf.Lerp(targetDistance, Mathf.Clamp(velocity.x, -1f, 1f) * distanceAhead, pullAheadSpeed * Time.deltaTime);
                targetPosition.x += targetDistance;
            }

            if (zoomOut)
            {
                RaycastHit2D hit = Physics2D.Raycast(target.position, Vector2.down, 1000f, groundLayer);

                if (!IsVisible(hit.point))
                {
                    targetPosition.z -= outStep * Time.deltaTime;

                    if (targetPosition.z < maxZoomDistance)
                        targetPosition.z = maxZoomDistance;
                }
                else
                    targetPosition.z = Mathf.Lerp(targetPosition.z, initialDistance, smoothIn * Time.deltaTime);
            }

            //Lerp camera position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        }
    }

    bool IsVisible(Vector3 point)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(point);

        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > padding && screenPoint.y < 1)
        {
            return true;
        }

        return false;
    }

    void Reposition()
    {
        //Position camera at player on respawn (prevents it flying back)
        targetPosition.x = target.position.x + offset.x;
        targetPosition.y = target.position.y + offset.y;

        transform.position = targetPosition;
    }
}

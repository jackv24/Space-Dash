using UnityEngine;
using System.Collections;
using InControl;

public class PlayerControl : MonoBehaviour
{
    //Input
    private InputDevice device;
    private float inputX;

    [Header("Movement")]
    //How fast the player can move horizontally
    public float moveSpeed = 10f;
    //How fast the player reaches it's move speed
    [Range(0f, 1f)]
    [Tooltip("How quickly the player reaches it's move speed. Set to 1 for instantly.")]
    public float acceleration = 0.75f;

    //The impulse force applied to make the player jump
    [Space()]
    public float jumpForce = 5f;
    [Tooltip("How many times the player can jump without touching the ground.")]
    public int jumpAmount = 2;
    private int jumpsLeft;
    private bool shouldJump = false;

    //What velocity of the rigidbody is set to
    private Vector2 moveVector;

    [Space()]
    [Tooltip("The speed at which the player's fall speed will stop increasing.\nSet to 0 to disable.")]
    public float terminalVelocity = -10f;

    [Header("Raycasting")]
    [Tooltip("The layer that is raycasted onto when checking if grounded.\nRemember to set the layer on ground objects")]
    public LayerMask groundLayer;
    [Tooltip("The distance from the ground at which the player is counted as 'grounded'")]
    public float groundedDistance = 0.01f;
    private bool isGrounded = false;

    [Space()]
    public float raysStartX = -0.5f;
    public float raysEndX = 0.5f;
    public int rayAmount = 3;

    //Reference to the player's rigidbody
    private Rigidbody2D body;

    void Awake()
    {
        //Get component references
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        device = InputManager.ActiveDevice;

        isGrounded = CheckGrounded();

        //Get inputs every frame
        if (device.Action1.WasPressed)
        {
            //Only jump if the player is grounded, or there are jumps left
            if(jumpsLeft > 0 || isGrounded)
                //Queue jump button (Inputs are handled every frame, but jump is only applied every physics step)
                shouldJump = true;
        }

        //Get X movement inputs (clamped)
        inputX = Mathf.Clamp(device.DPadX + device.LeftStickX, -1f, 1f);
    }

    //Update with the physics engine (since it is using a rigidbody)
    void FixedUpdate()
    {
        //Set horizontal movement of the vector
        moveVector.x = Mathf.Lerp(moveVector.x, inputX * moveSpeed, acceleration);

        //Preserve gravity, but limit fall velocity if necessary
        if (body.velocity.y > terminalVelocity || terminalVelocity == 0)
            moveVector.y = body.velocity.y;
        else
            moveVector.y = terminalVelocity;

        if (shouldJump)
        {
            //Use queued jump button
            shouldJump = false;
            //Decrement jumps left
            jumpsLeft--;

            //Set jump force (dont add, to prevent double jumps launching player)
            moveVector.y = jumpForce;
        }

        //Set velocity after moveVector is set
        body.velocity = moveVector;
    }

    //Checks if the character is grounded (using raycast)
    bool CheckGrounded()
    {
        RaycastHit2D[] hits = new RaycastHit2D[rayAmount];

        for (int i = 0; i < rayAmount; i++)
        {
            //Calculate ray origin
            float offsetX = Mathf.Lerp(raysStartX, raysEndX, i / (float)(rayAmount - 1));
            Vector2 origin = new Vector3(transform.position.x + offsetX, transform.position.y);

            //Fire ray
            hits[i] = Physics2D.Raycast(origin, Vector2.down, 100, groundLayer);

            //Display rays for debugging
            if(hits[i].collider != null)
                Debug.DrawLine(origin, hits[i].point, Color.green);
            else
                Debug.DrawLine(origin, origin + Vector2.down * 100, Color.red);

            if (hits[i].distance <= groundedDistance && hits[i].collider != null)
                return true;
        }

        //Otherwise, player is not grounded
        return false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //Reset jumps the player lands on ground
        if (isGrounded)
            jumpsLeft = jumpAmount;
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerControl : MonoBehaviour
{
    //Input
    private float inputX;

    [Header("Movement")]
    //How fast the player can move horizontally
    public float moveSpeed = 10f;

    //How fast the player reaches it's move speed
    [Range(0f, 1f)]
    [Tooltip("How quickly the player reaches it's move speed. Set to 1 for instantly.")]
    public float acceleration = 0.75f;
    public enum AutoRunDirection
    {
        Left, Right, None
    }
    [Space()]
    [Tooltip("Simulates axis input to make the player automatically move in the specified direction.")]
    public AutoRunDirection autoRun = AutoRunDirection.None;

    //The impulse force applied to make the player jump
    [Space()]
    public float jumpForce = 5f;
    [Tooltip("How many times the player can jump without touching the ground.")]
    public int jumpAmount = 2;
    public int jumpsLeft;
    private bool shouldJump = false;

    [Space()]
    [Tooltip("The speed at which the player falls when floating.")]
    public float floatingFallSpeed = -1f;
    [Tooltip("How much oxygen is used per second when floating.")]
    public float floatingOxygenUsage = 5f;
    public bool isFloating = false;

    [Space()]
    public ParticleSystem floatingParticles;

    //What velocity of the rigidbody is set to
    private Vector2 moveVector;

    private float terminalVelocity = 0;

    [Header("Raycasting")]
    [Tooltip("The layer that is raycasted onto when checking if grounded.\nRemember to set the layer on ground objects.")]
    public LayerMask groundLayer;

    [Tooltip("The distance from the ground at which the player is counted as 'grounded'.")]
    public float groundedDistance = 0.01f;
    private bool isGrounded = false;

    [Space()]
    public float raysStartX = -0.5f;
    public float raysEndX = 0.5f;
    public int rayAmount = 3;

    //Reference to the player's rigidbody
    private Rigidbody2D body;

    private PlayerStats playerStats;

    private CameraFollow cameraFollow;

    void Awake()
    {
        //Get component references
        body = GetComponent<Rigidbody2D>();

        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        StartCoroutine("UseOxygen");
    }

    void Update()
    {
        if (cameraFollow)
            cameraFollow.velocity = moveVector;

        //Check if the player is grounded every frame
        isGrounded = CheckGrounded();

        //Can only move if player is alive
        if (playerStats.IsAlive)
        {
            //Get inputs every frame
            if (Input.GetButtonDown("Jump"))
            {
                //Only jump if the player is grounded, or there are jumps left
                if (jumpsLeft > 0 || isGrounded)
                    //Queue jump button (Inputs are handled every frame, but jump is only applied every physics step)
                    shouldJump = true;
            }

            //Autorun simulates axis input
            if (autoRun == AutoRunDirection.Right)
                inputX = 1;
            else if (autoRun == AutoRunDirection.Left)
                inputX = -1;
            //Get X movement inputs (clamped)
            else
                inputX = Input.GetAxisRaw("Horizontal");

            //Only float if falling while jump button is held
            isFloating = (body.velocity.y < 0 && Input.GetButton("Jump"));
        }
        else
            inputX = 0;

        //Starts and stop particle system at the start and end of floating
        if (isFloating && floatingParticles.isStopped)
            floatingParticles.Play();
        else if(!isFloating && !floatingParticles.isStopped)
            floatingParticles.Stop();

        //Debugging
        if (Input.GetKeyDown(KeyCode.F) && Application.isEditor)
            Time.timeScale = 5f;
        else if (Input.GetKeyUp(KeyCode.F))
            Time.timeScale = 1f;
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

        //Set terminal velocity if isFloatig
        terminalVelocity = (isFloating && playerStats.IsAlive) ? floatingFallSpeed : 0;

        //Set velocity after moveVector is set
        body.velocity = moveVector;

        //If grounded and not moving up, reset jumps
        if (isGrounded && moveVector.y <= 0)
            jumpsLeft = jumpAmount;
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

    IEnumerator UseOxygen()
    {
        //this coruotine should always be running
        while (true)
        {
            //If floating then use oxygen
            if (isFloating)
            {
                //For every amount of time per second
                yield return new WaitForSeconds(1/floatingOxygenUsage);
                //Remove 1 oxygen
                playerStats.RemoveOxygen(1);
            }
            //If not floating then check again next frame
            else
                yield return new WaitForEndOfFrame();
        }
    }
}

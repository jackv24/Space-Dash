using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerControl : MonoBehaviour
{
    //Input
    private float inputX;

    [Header("Movement")]
    [Tooltip("How fast the player can move horizontally.")]
    public float moveSpeed = 10f;
    private float currentMoveSpeed;
    [Tooltip("How far until the player stops accelerating (in metres).")]
    public int maxAccelerationDistance = 2000;
    [Tooltip("How much speed the player gains until the max distance.")]
    public AnimationCurve acceleration;

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
    private int startJumps;
    [HideInInspector]
    public int jumpsLeft;
    private bool shouldJump = false;
    [Tooltip("The time after jumping before it can reset jumps - fixes jumping bug.")]
    public float jumpResetDelay = 0.5f;
    private float jumpResetTime = 0;

    [Space()]
    [Tooltip("The speed at which the player falls when floating.")]
    public float floatingFallSpeed = 0f;
    [Tooltip("How much oxygen is used per second when floating.")]
    public float floatingOxygenUsage = 5f;
    [HideInInspector]
    public bool isFloating = false;
    //Track whether the player has started falling, so boosting can occur
    private bool startedFalling = false;
    private bool shouldFloat = false;
    [Tooltip("How many seconds after the player starts falling boosting starts (prevents oxygen being used while jumping).")]
    public float startBoostDelay = 0.25f;
    private float startBoostTime;

    [Space()]
    [Tooltip("Player will not be able to boost above this height. Please note that this is not the height above the ground, but simply the player's height on the y axis.")]
    public float maxHeight = 50f;
    private float currentFallSpeed = 0f;

    [Space()]
    public GameObject floatingParticles;
    private GameObject lastFloatingParticles;
    public GameObject runParticles;
    public GameObject landParticles;
    public GameObject jumpParticles;

    //What velocity of the rigidbody is set to
    private Vector2 moveVector;

    private float terminalVelocity = 0;

    [Header("Raycasting")]
    [Tooltip("The layer that is raycasted onto when checking if grounded.\nRemember to set the layer on ground objects.")]
    public LayerMask groundLayer;

    [Tooltip("The distance from the ground at which the player is counted as 'grounded'.")]
    public float groundedDistance = 0.01f;
    private bool isGrounded = false;
    private bool becameGrounded = false;

    [Space()]
    public float raysStartX = -0.5f;
    public float raysEndX = 0.5f;
    public int rayAmount = 3;

    [Space()]
    public Animator anim;

    //Reference to the player's rigidbody
    private Rigidbody2D body;
    private Vector2 oldVelocity;
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

        startJumps = jumpAmount;
        currentMoveSpeed = moveSpeed;
        currentFallSpeed = floatingFallSpeed;

        StartCoroutine("UseOxygen");

        //Subscribe to pause events
        GameManager.instance.OnGamePaused += PausePhysics;
        GameManager.instance.OnGameResumed += ResumePhysics;
    }

    void Update()
    {
        if (cameraFollow)
            cameraFollow.velocity = moveVector;

        //Check if the player is grounded every frame
        isGrounded = CheckGrounded();

        if (anim)
            anim.SetBool("grounded", isGrounded);

        //Can only move if player is alive
        if (playerStats.IsAlive && GameManager.instance.IsGamePlaying && !GameManager.instance.IsGamePaused)
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

            //Adjust move speed according to acceleration curve
            currentMoveSpeed = moveSpeed + acceleration.Evaluate((transform.position.x < maxAccelerationDistance) ? transform.position.x / maxAccelerationDistance : 1);

            //Keeps track of when the player started falling, and when they can boost
            if ((!startedFalling || shouldJump) && body.velocity.y < 0 && jumpsLeft < jumpAmount)
            {
                startedFalling = true;
                startBoostTime = Time.time + startBoostDelay;
            }

            //Only float if falling while jump button is held
            isFloating = (!shouldJump && shouldFloat && startedFalling && Input.GetButton("Jump"));

            if (Input.GetButton("Jump") && Time.time > startBoostTime)
                shouldFloat = true;
            if (Input.GetButtonUp("Jump"))
                shouldFloat = false;

            if (isGrounded && startedFalling)
                startedFalling = false;
        }
        else
        {
            //Dont move
            inputX = 0;

            isFloating = false;

            //Stop floating particles if player can not move (prevents them getting stuck on if boosting before death)
            if (lastFloatingParticles)
            {
                lastFloatingParticles.GetComponent<ParticleSystem>().Stop();
                lastFloatingParticles = null;
            }
        }

        //Starts and stop particle system at the start and end of floating
        if (isFloating && !lastFloatingParticles)
        {
            lastFloatingParticles = (GameObject)Instantiate(floatingParticles, floatingParticles.transform.position, floatingParticles.transform.rotation);
            lastFloatingParticles.transform.parent = transform;
            lastFloatingParticles.SetActive(true);

            if (SoundManager.instance)
                SoundManager.instance.SetPlayerLoop(SoundManager.instance.sounds.boosting);
        }
        else if (!isFloating && lastFloatingParticles)
        {
            lastFloatingParticles.GetComponent<ParticleSystem>().Stop();
            lastFloatingParticles = null;

            if (SoundManager.instance)
                SoundManager.instance.SetPlayerLoop(null);
        }

        //Debugging
        if (Input.GetKeyDown(KeyCode.F) && Application.isEditor)
            Time.timeScale = 5f;
        else if (Input.GetKeyUp(KeyCode.F))
            Time.timeScale = 1f;
    }

    public void PausePhysics()
    {
        oldVelocity = body.velocity;
        body.isKinematic = true;
        isFloating = false;
    }

    public void ResumePhysics()
    {
        body.isKinematic = false;
        body.velocity = oldVelocity;
    }

    //Update with the physics engine (since it is using a rigidbody)
    void FixedUpdate()
    {
        //If grounded and not moving up, reset jumps
        if (isGrounded && Time.time > jumpResetTime)
        {
            jumpsLeft = jumpAmount;
            jumpResetTime = Time.time + jumpResetDelay;
        }

        //Set horizontal movement of the vector
        moveVector.x = inputX * currentMoveSpeed;

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

            //Play jumping sound if jumping from ground, and air jumping sound if jumping from air
            if(SoundManager.instance)
                SoundManager.instance.PlaySound(SoundManager.instance.sounds.RandomJump);

            if (jumpParticles)
                Instantiate(jumpParticles, transform.position, jumpParticles.transform.rotation);

            //Set jump force (dont add, to prevent double jumps launching player)
            moveVector.y = jumpForce;

            if (anim)
                anim.SetTrigger("jump");
        }

        //Cap player's height
        if (transform.position.y > maxHeight)
            currentFallSpeed = 0f;
        else
            currentFallSpeed = floatingFallSpeed;

        moveVector.y = isFloating ? currentFallSpeed : moveVector.y;

        if (anim)
            anim.SetBool("boosting", isFloating);

        //Set velocity after moveVector is set
        body.velocity = moveVector;

        if (anim)
            anim.SetFloat("speed", moveVector.x);
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
            {
                if(!becameGrounded && playerStats.IsAlive && moveVector.x > 0 && GameManager.instance.IsGamePlaying)
                {
                    becameGrounded = true;

                    //Start footstep sound if just became grounded
                    if (SoundManager.instance)
                    {
                        SoundManager.instance.PlaySound(SoundManager.instance.sounds.landing);
                        SoundManager.instance.SetPlayerLoop(SoundManager.instance.sounds.running);
                    }

                    if (landParticles)
                        Instantiate(landParticles, transform.position, landParticles.transform.rotation);

                    if (runParticles)
                        runParticles.SetActive(true);
                }

                return true;
            }
        }

        if (becameGrounded)
        {
            becameGrounded = false;

            if (SoundManager.instance)
                SoundManager.instance.SetPlayerLoop(null);

            if (runParticles)
                runParticles.SetActive(false);
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

    public void AddJump(bool resetJumps)
    {
        jumpAmount++;

        if (resetJumps)
            jumpsLeft = jumpAmount;
        else
            jumpsLeft++;
    }

    public void Reset()
    {
        jumpAmount = startJumps;
        currentMoveSpeed = moveSpeed;
    }
}

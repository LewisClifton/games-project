using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private GameInput gameInput;

    [Header("Movement")]
    [Header("Walking")]
    public float moveSpeed;
    private float runSpeed;
    
    public Transform orientation;
    public int walkingDrag;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    public int airDrag;
    Rigidbody playerBody;
    Vector2 moveInput;
    public float airMoveSpeed;
    [Header("Free Dash")]
    public float freeDashForce;
    public float dashUpwardForce;
    public float dashDuration;
    [Header("Attack Dash")]
    //public Transform enemy;
    public float attackDashForce;
    public float detectionRange;
    public GameObject playerObject;
    public float dashTime;
    LayerMask originalLayer;
    [Header("Player State")]
    public bool isGrounded = false;
    public bool isGliding = false;
    
    [Header("Player State Checking")]
    private float groundedCheckDistance = 1.5f;
    private float colliderHeight;
    private float bufferCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Collider playerCollider;
    [Header("Gliding")]
    [SerializeField] private float BaseSpeed;
    [SerializeField] private float MaxThrustSpeed;// Max thrust force
    [SerializeField] private float MinThrustSpeed;// Minimum speed required for gliding thrust
    [SerializeField] private Transform cameraTransform;// Camera, set to Main Camera
    [SerializeField] private float ThrustFactor;// Used to determine thrust force
    [SerializeField] private Vector3 upwardForce; // Force that is applied to player continuously when gliding. To immitate lower gravity.
    [SerializeField] private float glideRunSpeed;
    [SerializeField] private bool glidingEnabled;
    private float CurrentThrustSpeed;
    private float attackDashCooldown;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject character;
    

    void Start()
    {
        originalLayer = playerObject.layer;
        playerBody = GetComponent<Rigidbody>();
        playerBody.freezeRotation = true;
        colliderHeight = playerCollider.bounds.size.y;
        runSpeed = moveSpeed;
    }

    
    void Update()
    {
        attackDashCooldown=attackDashCooldown-Time.deltaTime;
        if (attackDashCooldown<0){
            attackDashCooldown=0;
        }
        Shader.SetGlobalVector("_Player", transform.position);

        if (gameInput.IsAttackDashPressed())
        {
            if (attackDashCooldown==0){
            AttackDash();
            }
        }
        if (gameInput.IsFreeDashPressed())
        {
            FreeDash();
        }
        Jump();
        if (!isGrounded && !isGliding)
        {
            moveSpeed = airMoveSpeed;
        }
        else
        {
            if (isGrounded)
            {
                moveSpeed = runSpeed;
            }
        }
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        checkGroundStatus();
        if (!isGrounded && isGliding && glidingEnabled)
        {
            GlidingMovement();
            ManageRotation();
        };

    }
    

    private void MovePlayer() { 
        Vector3 playerVelocity = orientation.forward * moveInput.y * moveSpeed + orientation.right * moveInput.x * moveSpeed;
        playerBody.AddForce(playerVelocity, ForceMode.Force);
        //playerBody.velocity = transform.TransformDirection(playerVelocity);
        if (playerVelocity != Vector3.zero)
        {
            character.transform.rotation = Quaternion.LookRotation(playerVelocity);
        }
    }

    //Function gets moveInput value, it is run but vscode just can't tell.
    private void OnMove(InputValue value)
    {
        moveInput=value.Get<Vector2>();
        animator.SetFloat("movementX", moveInput.normalized.x);
        animator.SetFloat("movementZ", moveInput.normalized.y);
    }

    //Ran every frame. Checks if player begins a jump.
    private void Jump()
    {
        if (gameInput.IsJumpPressed())
        {
            if (isGrounded)
            {

                playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animator.SetBool("inAir", true);
            }
            else
            {
                if (isGliding)
                {
                    moveSpeed = runSpeed;
                }
                else
                {
                    moveSpeed = glideRunSpeed;
                }
                isGliding = !isGliding;
            }
        }
    }

    //Function ran everytime the player lands on the ground.
    private void onLand()
    {
        // Makes player stand up
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        isGliding = false;

        // Reset the drag to 0
        // playerBody.drag = 0;
        // Reset the movement speed to running speed
        //movementSpeed = runningSpeed;
        playerBody.drag = walkingDrag;
        moveSpeed = runSpeed;
        
    }
    private void onTakingAir()
    {
        playerBody.drag = airDrag;
    }

    //Ran every FixedUpdate, checks if the player is on the ground.
    private void checkGroundStatus()
    {
        groundedCheckDistance = (colliderHeight / 2) + bufferCheckDistance;

        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down;

        RaycastHit hit;
        if (Physics.Raycast(rayStart, rayDirection, out hit, groundedCheckDistance, groundLayer))
        {
            //Checks if the player has "landed"
            if (!isGrounded)
            {
                animator.SetBool("inAir", false);
                onLand();
            }
            isGrounded = true;

            
        }
        else
        {
            if (isGrounded)
            {
                onTakingAir();
            }
            isGrounded = false;
        }
    }

    

    //Function that executes the dash
    private void FreeDash()
    {
        Vector3 forceToApply = Vector3.zero;
        if (moveInput != Vector2.zero)
        {
            forceToApply = orientation.transform.forward * freeDashForce * moveInput.y + orientation.transform.right * freeDashForce * moveInput.x;
        }
        else
        {
            forceToApply = orientation.transform.forward * freeDashForce + orientation.transform.up * dashUpwardForce;
        }
        playerBody.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void AttackDash()
    {
        attackDashCooldown=1;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(orientation.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        if (closestDistance < detectionRange)
        {
            playerObject.layer = LayerMask.NameToLayer("Dashing");
            Vector3 playerVelocity=playerBody.velocity;
            float playerSpeed = playerVelocity.magnitude;
            playerBody.AddForce(playerVelocity * -1, ForceMode.Impulse);
            Vector3 forceToApply = Vector3.zero;
            Vector3 vectorToEnemy = closestEnemy.transform.position - orientation.transform.position;
            //vectorToEnemy= vectorToEnemy.normalized;
            forceToApply = vectorToEnemy * (attackDashForce) + vectorToEnemy.normalized*playerSpeed;
            //Debug.Log(vectorToEnemy);
            playerBody.AddForce(forceToApply, ForceMode.Impulse);
            Invoke("setLayer",dashTime);
        }
        else
        {
            Debug.Log(closestDistance);
        }
        
    }
    
    private void setLayer()
    {
        playerObject.layer = originalLayer;
    }
    private void GlidingMovement()
    {
        
        float pitchInRads = transform.eulerAngles.x * Mathf.Deg2Rad;
        //Pitch gets mapped to a value between -1 and 1 using Sin and multiplied by the thrust factor.
        //If the "nose" of the character points up then it slows down, if it points down then it speeds up.
        float mappedPitch = Mathf.Sin(pitchInRads) * ThrustFactor;
        Vector3 glidingForce = Vector3.forward * CurrentThrustSpeed;
        //Time.deltaTime is used to account for changes in framerate/frametime
        CurrentThrustSpeed += mappedPitch * Time.deltaTime;
        //Forces the CurrentThrustSpeed to be between 0 and MaxThrustSpeed
        CurrentThrustSpeed = Mathf.Clamp(CurrentThrustSpeed, 0, MaxThrustSpeed);
        //Checks if the speed of the player is high enough for gliding forces to be applied
        if (playerBody.velocity.magnitude >= MinThrustSpeed)
        {
            //Applies glidingForce to the direction the player is pointing
            playerBody.AddRelativeForce(glidingForce);
        }
        else
        {
            //If the player is below the threshold speed then the CurrentThrustSpeed is reset to 0
            CurrentThrustSpeed = 0;
        }
        playerBody.AddForce(upwardForce, ForceMode.Force);
    }

    private void ManageRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        transform.rotation = targetRotation;

    }
}

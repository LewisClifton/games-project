using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private GameInput gameInput;

    [Header("Movement")]
    [Header("Walking")]
    public float moveSpeed;
    private float runSpeed;
    private float originalRunSpeed;
    public float currentAcceleration=0;
    public float maxAcceleration = 5;
    public float decelerationRate=5;
    public float slopeDetectionDistance = 3;
    public Transform playerRotation;
    public Dictionary<int, int> multiplierToSpeedConversions = new Dictionary<int, int>
    {
        { 1, 0 },
        { 2, 20 },
        { 3, 50 },
        { 4, 100 },
        { 5, 150 }
    };

    public Transform orientation;
    public int walkingDrag;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    public int airDrag;
    Rigidbody playerBody;
    Vector2 moveInput;
    public float airMoveSpeed;
    public float extraGravity=0.5f;
    private float originalExtraGrav;
    [Header("Free Dash")]
    public float freeDashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float freeDashCooldown = 0.4f;
    private float currentFreeDashCooldown=0;
    [Header("Attack Dash")]
    //public Transform enemy;
    public float attackDashForce;
    public float detectionRange;
    public GameObject playerObject;
    public float dashTime = 1;
    public float attackDashCooldown =0.2f;
    private float currentAttackDashCooldown;
    public float fieldOfViewAngle = 140;
    public float attackDashVerticalFactor;

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
    [Header("Camera Lock On")]
    public CinemachineVirtualCamera lockOnCamera;
    private CinemachineTransposer transposer;
    private GameObject target;
    private bool lockedOn = false;
    public int lockedOnDistance;
    public int lockedOnHeight;
    public int lockOnRange;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject character;
    float preDrag;

    void Awake()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        originalLayer = playerObject.layer;
        playerBody = GetComponent<Rigidbody>();
        playerBody.freezeRotation = true;
        colliderHeight = playerCollider.bounds.size.y;
        runSpeed = moveSpeed;
        originalRunSpeed = runSpeed;
        transposer = lockOnCamera.GetCinemachineComponent<CinemachineTransposer>();
        originalExtraGrav = extraGravity;
    }


    void Update()
    {
        if (lockedOn)
        {
            if (lockOnCamera.LookAt != null)
            {
                AdjustCameraBasedOnPlayerPosition();
            }
            else
            {
                print("HERE");
                target = null;
                lockedOn = false;
                lockOnCamera.enabled = false;
                lockOnCamera.LookAt = null;
            }
        }
        if (currentAttackDashCooldown != 0)
        {
            currentAttackDashCooldown = currentAttackDashCooldown - Time.deltaTime;
            if (currentAttackDashCooldown < 0)
            {
                currentAttackDashCooldown = 0;
            }
        }
        
        if (currentFreeDashCooldown != 0)
        {
            currentFreeDashCooldown-= Time.deltaTime;
            if (currentFreeDashCooldown < 0) 
            { 
                currentFreeDashCooldown = 0;
            }
        }
        Shader.SetGlobalVector("_Player", transform.position);

        if (gameInput.IsLockOnPressed())
        {
            if (!lockedOn)
            {

                FindTarget();
                if (target != null)
                {
                    lockedOn = true;
                    lockOnCamera.enabled = true;
                }
            }
            else
            {
                target = null;
                lockedOn = false;
                lockOnCamera.enabled = false;
                lockOnCamera.LookAt=null;
            }
        }
        if (gameInput.IsAttackDashPressed())
        {
            if (currentAttackDashCooldown == 0)
            {
                AttackDash();
            }
        }
        if (gameInput.IsFreeDashPressed())
        {
            if (currentFreeDashCooldown == 0)
            {
                FreeDash();
                currentFreeDashCooldown = freeDashCooldown;
            }
        }
        Jump();
        

    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            playerBody.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
        MovePlayer();
        checkGroundStatus();
        if (!isGrounded && isGliding && glidingEnabled)
        {
            GlidingMovement();
            ManageRotation();
        };

    }

    private void LateUpdate()
    {
        
    }
    private void MovePlayer()
    {
        if (moveInput == Vector2.zero)
        {
            currentAcceleration = Mathf.Clamp(currentAcceleration - decelerationRate*Time.fixedDeltaTime, 0, maxAcceleration);
        }
        else
        {
            currentAcceleration = Mathf.Clamp(currentAcceleration + Time.fixedDeltaTime, 0, maxAcceleration);
        }
        Vector3 playerVelocity = (orientation.forward * moveInput.y * moveSpeed + orientation.right * moveInput.x * moveSpeed)*((1+(currentAcceleration/maxAcceleration))/2);
        RaycastHit hit;
        if (Physics.Raycast(playerBody.transform.position,Vector3.down, out hit, slopeDetectionDistance))
        {
            Vector3 groundNormal=hit.normal;
            float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);
            //Debug.Log(slopeAngle);
            playerVelocity = Vector3.ProjectOnPlane(playerVelocity, groundNormal);
        }
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
        moveInput = value.Get<Vector2>();
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
        //Vector3 rayDirection = Vector3.down;
        Vector3 rayDirection = -playerRotation.transform.up;
        rayDirection= rayDirection.normalized;
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
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Vector3 vectorToEnemy = enemy.transform.position - transform.position;
            float angleToEnemy = Vector3.Angle(orientation.forward, vectorToEnemy);
            if (angleToEnemy <= fieldOfViewAngle / 2)
            {
                float distance = Vector3.Distance(orientation.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

        }
        if (closestDistance < detectionRange)
        {
            moveSpeed = 0;
            extraGravity = -9.8f * playerBody.mass;
            //playerBody.drag = 5;
            currentAttackDashCooldown = attackDashCooldown;
            playerObject.layer = LayerMask.NameToLayer("Dashing");
            Vector3 playerVelocity = playerBody.velocity;
            float playerSpeed = playerVelocity.magnitude;
            //playerBody.AddForce(playerVelocity * -1, ForceMode.Impulse);
            playerBody.velocity = Vector3.zero;
            Vector3 forceToApply = Vector3.zero;
            Vector3 vectorToEnemy = closestEnemy.transform.position - orientation.transform.position;
            //vectorToEnemy= vectorToEnemy.normalized;
            forceToApply = vectorToEnemy * (attackDashForce) + vectorToEnemy.normalized * playerSpeed;
            //Debug.Log(vectorToEnemy);
            forceToApply.y = forceToApply.y + Mathf.Abs(forceToApply.y) * attackDashVerticalFactor;
            playerBody.AddForce(forceToApply, ForceMode.Impulse);
            Invoke("setLayer", dashTime);
        }
        else
        {
            Debug.Log(closestDistance);
        }

    }

    private void setLayer()
    {
        playerObject.layer = originalLayer;
        multiplierToSpeedConversions.TryGetValue(ScoreManager.instance.GetMultiplier(), out int currMultAdd);
        runSpeed = originalRunSpeed + currMultAdd;
        extraGravity = originalExtraGrav;
        moveSpeed = runSpeed;
        //if (isGrounded)
        //{
        //    playerBody.drag = walkingDrag;
        //}
        //else
        //{
        //    playerBody.drag = airDrag;
        //}
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
    void AdjustCameraBasedOnPlayerPosition()
    {
        Vector3 playerToBoss = target.transform.position - playerBody.transform.position;
        //float angle = Vector3.SignedAngle(playerToBoss, transform.forward, Vector3.up);
        playerToBoss.Normalize();
        playerToBoss.y = 0;
        playerToBoss = playerToBoss * lockedOnDistance;
        playerToBoss.y += lockedOnHeight;
        transposer.m_FollowOffset = new Vector3(-playerToBoss.x, playerToBoss.y, -playerToBoss.z);
        lockOnCamera.LookAt = target.transform;
        lockOnCamera.Follow = playerBody.transform;
    }
    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(orientation.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                Vector3 vectorToEnemy = enemy.transform.position - transform.position;
                float angleToEnemy = Vector3.Angle(orientation.forward, vectorToEnemy);
                if (angleToEnemy <= fieldOfViewAngle / 2)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        if (closestDistance < lockOnRange)
        {
            target = closestEnemy;
            lockOnCamera.LookAt = target.transform;
        }
        else
        {
            target = null;
        }
    }
}

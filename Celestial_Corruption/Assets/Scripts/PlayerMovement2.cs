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
    public Transform orientation;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    Rigidbody playerBody;
    Vector2 moveInput;
    [Header("Free Dash")]
    public float freeDashForce;
    public float dashUpwardForce;
    public float dashDuration;
    [Header("Attack Dash")]
    public Transform enemy;
    public float attackDashForce;
    [Header("Player State")]
    public bool isGrounded = false;

    [Header("Player State Checking")]
    private float groundedCheckDistance = 1.5f;
    private float colliderHeight;
    private float bufferCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Collider playerCollider;



    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        playerBody.freezeRotation = true;
        colliderHeight = playerCollider.bounds.size.y;
    }

    
    void Update()
    {
        Shader.SetGlobalVector("_Player", transform.position);

        
        Jump();
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
        checkGroundStatus();
        if (gameInput.IsFreeDashPressed())
        {
            FreeDash();
        }
        if (gameInput.IsAttackDashPressed())
        {
            AttackDash();
        }
    }


    private void MovePlayer() { 
        Vector3 playerVelocity = orientation.forward * moveInput.y * moveSpeed + orientation.right * moveInput.x * moveSpeed;
        playerBody.AddForce(playerVelocity, ForceMode.Force);
        //playerBody.velocity = transform.TransformDirection(playerVelocity);
        
    }

    //Function gets moveInput value, it is run but vscode just can't tell.
    private void OnMove(InputValue value)
    {
        moveInput=value.Get<Vector2>();
    }

    //Ran every frame. Checks if player begins a jump.
    private void Jump()
    {
        if (gameInput.IsJumpPressed() && isGrounded)
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    //Function ran everytime the player lands on the ground.
    private void onLand()
    {
        // Makes player stand up
        //transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        //isGliding = false;

        // Reset the drag to 0
        // playerBody.drag = 0;
        // Reset the movement speed to running speed
        //movementSpeed = runningSpeed;
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
                onLand();
            }
            isGrounded = true;
        }
        else
        {
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
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {

    }

    private void AttackDash()
    {
        Vector3 forceToApply = Vector3.zero;
        Vector3 vectorToEnemy = enemy.transform.position - orientation.transform.position;
        //vectorToEnemy= vectorToEnemy.normalized;
        forceToApply = vectorToEnemy * attackDashForce;
        playerBody.AddForce(forceToApply, ForceMode.Impulse);
    }
}

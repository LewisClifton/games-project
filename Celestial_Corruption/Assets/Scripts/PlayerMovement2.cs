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
    public float moveSpeed;
    public Transform orientation;
    [SerializeField] private float jumpForce = 5f;
    Rigidbody playerBody;
    Vector2 moveInput;

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
        MovePlayer();
        Jump();
    }

    private void FixedUpdate()
    {
        checkGroundStatus();
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
}

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Player : MonoBehaviour
{   
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Collider playerCollider;

    // Layermask doesn't have to be only ground, it can be any layer you want the player to be able to stand on
    [SerializeField] private LayerMask groundLayer;

    // Normal movement variables 

    [SerializeField] private float runningSpeed = 15f;
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 5f;


    // Gliding movement variables

    [SerializeField] private float BaseSpeed = 30;
    // Max thrust force
    [SerializeField] private float MaxThrustSpeed = 400;
    // Minimum speed required for gliding thrust
    [SerializeField] private float MinThrustSpeed = 0;
    [SerializeField] private float ThrustFactor = 80;
    private float groundedCheckDistance = 1.1f;
    private float bufferCheckDistance = 0.5f;
    
    private float movementSpeed;
    private float CurrentThrustSpeed;
    private float colliderHeight;
    
    // Player flags
    private bool isWalking = false;
    public bool isGliding = false;
    public bool isGrounded = false;


    private void Awake()
    {
        // Set the movement speed to running speed by default
        movementSpeed = runningSpeed;

        // Makes cursor invisible and locks it to centre of screen (esc during play mode to display it again)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        colliderHeight = playerCollider.bounds.size.y;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Shader.SetGlobalVector("_Player", transform.position);
        checkGroundStatus();
        ToggleGlide();
        if (isGliding)
        {
            GlidingMovement();
            ManageRotation();
        }
        else
        {
            NormalMovement();
        }
    }

    private void NormalMovement()
    {
        ToggleWalk();
        Move();
        Jump();
    }

    private void GlidingMovement()
    {
        float pitchInRads = transform.eulerAngles.x * Mathf.Deg2Rad;
        //Pitch gets mapped to a value between -1 and 1 using Sin and multiplied by the thrust factor.
        //If the "nose" of the character points up then it slows down, if it points down then it speeds up.
        float mappedPitch=Mathf.Sin(pitchInRads) *ThrustFactor;
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
    }

    //Uses camera rotation to dictate character rotation and direction for forces to be applied
    private void ManageRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        transform.rotation = targetRotation;
    }

    // Simple Movement with rotation and relative to the camera orientation
    private void Move()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Convert the input vector to a movement vector relative to the camera orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Adjusted movement vector
        Vector3 movementVector = (forward * inputVector.y + right * inputVector.x).normalized;
        movementVector *= movementSpeed * Time.deltaTime;

        // Move the player 
        playerBody.MovePosition(playerBody.position + movementVector);

        // Rotate the player
        if (movementVector != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementVector);
            playerBody.MoveRotation(Quaternion.RotateTowards(playerBody.rotation, toRotation, rotationSpeed * Time.deltaTime));
        }
    }

    // Simple Jump
    // TODO: Add raycast to check if the player is grounded
    private void Jump()
    {   
        if (gameInput.IsJumpPressed() && isGrounded)
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    // Simple Toggle between walking and running
    private void ToggleWalk()
    {
        if (gameInput.IsWalkTogglePressed())
        {
            isWalking = !isWalking;
            // Change the movement speed only if WalkToggle is pressed
            movementSpeed = isWalking ? runningSpeed : walkingSpeed;
        }
    }

    private void ToggleGlide()
    {
        if (gameInput.IsJumpPressed() && !isGrounded)
        {
            isGliding = !isGliding;
            // Change the movement speed only if GlideToggle is pressed
            movementSpeed = isGliding ? BaseSpeed : runningSpeed;
        }
    }

    //This function activates when isGrounded goes from False to True
    private void onLand()
    {
        //Makes player stand up
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        isGliding = false;
    }

    //Function used to check if the player is grounded
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

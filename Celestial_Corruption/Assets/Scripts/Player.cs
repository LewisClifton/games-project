using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    
    [SerializeField] private float runningSpeed = 15f;
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerBody;

    // True if the player is walking, false if the player is running
    // Character is running by default
    private bool isWalking = false;
    private float movementSpeed;

    private void Awake()
    {
        // Set the movement speed to running speed by default
        movementSpeed = runningSpeed;
    }

    private void Update()
    {
        ToggleWalk();
        Move();
        Jump();
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
        if (gameInput.IsJumpPressed())
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Simple Toggle between walking and running
    // No detection for jump or other actions
    private void ToggleWalk()
    {
        if (gameInput.IsWalkTogglePressed())
        {
            isWalking = !isWalking;
            // Change the movement speed only if WalkToggle is pressed
            movementSpeed = isWalking ? runningSpeed : walkingSpeed;
        }
    }
}

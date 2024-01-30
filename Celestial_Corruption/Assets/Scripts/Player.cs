using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerBody;

    private bool isWalking;

    void Update()
    {
        Move();  
    }

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
}

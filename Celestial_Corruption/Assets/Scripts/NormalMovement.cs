using UnityEngine;

public class NormalMovement : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Collider playerCollider;

    // Layermask doesn't have to be only ground, it can be any layer you want the player to be able to stand on
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float runningSpeed = 15f;
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private float groundedCheckDistance = 1.1f;
    private float bufferCheckDistance = 0.5f;

    private float movementSpeed;
    private float colliderHeight;

    private bool isWalking = false;

    private void Awake()
    {
        // Done in the Player.cs script
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        movementSpeed = runningSpeed;
        // colliderHeight = playerCollider.bounds.size.y;
        colliderHeight = 2.0f;
    }

    private void Start()
    {

    }

    private void Update()
    {
        // Check if the player is grounded
        if (isGrounded())
        {
            MovePlayer();
            Jump();
            SetWalking();
        } else 
        {
            // If player is not grounded and the jump button is pressed, set the player state to gliding
            if (gameInput.IsJumpPressed() && !isGrounded())
            {
                playerController.SetPlayerState(PlayerState.Gliding);
            }
        }
    }

    private void MovePlayer()
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

    private void Jump()
    {
        if (gameInput.IsJumpPressed() && isGrounded())
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void SetWalking()
    {
        if (gameInput.IsWalkTogglePressed())
        {
            isWalking = !isWalking;
            // Change the movement speed only if WalkToggle is pressed
            movementSpeed = isWalking ? runningSpeed : walkingSpeed;
        }
    }

    private bool isGrounded()
    {
        return Physics.Raycast(playerBody.transform.position, Vector3.down, colliderHeight / 2 + groundedCheckDistance, groundLayer);
    }
}
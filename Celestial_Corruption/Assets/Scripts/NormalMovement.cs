using UnityEngine;

public class NormalMovement : MonoBehaviour
{
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
    }

    private void Start()
    {

    }

    private void Update()
    {
        MovePlayer();
        Jump();
    }

    private void MovePlayer()
    {
        // Get the input from the player
        float horizontalInput = gameInput.GetHorizontalInput();
        float verticalInput = gameInput.GetVerticalInput();

        // Get the direction the player is facing
        Vector3 moveDirection = cameraTransform.forward * verticalInput + cameraTransform.right * horizontalInput;
        moveDirection.y = 0;

        // Rotate the player to face the direction they are moving
        if (moveDirection != Vector3.zero)
        {
            playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
        }

        // Move the player
        playerBody.velocity = moveDirection * movementSpeed + new Vector3(0, playerBody.velocity.y, 0);
    }

    private void Jump()
    {
        if (gameInput.IsJumpPressed() && isGrounded())
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool isGrounded()
    {
        return Physics.Raycast(playerBody.transform.position, Vector3.down, colliderHeight / 2 + groundedCheckDistance, groundLayer);
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void SetWalking(bool walking)
    {
        isWalking = walking;
        movementSpeed = isWalking ? walkingSpeed : runningSpeed;
    }



}
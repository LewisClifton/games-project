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

    [Header("Player State")]
    public bool isGrounded = false;


    Vector2 moveInput;


    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        playerBody.freezeRotation = true;
    }

    
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer() { 
        Vector3 playerVelocity = orientation.forward * moveInput.y * moveSpeed + orientation.right * moveInput.x * moveSpeed;
        playerBody.AddForce(playerVelocity, ForceMode.Force);
        //playerBody.velocity = transform.TransformDirection(playerVelocity);
    }

    private void OnMove(InputValue value)
    {
        moveInput=value.Get<Vector2>();
    }

    private void Jump()
    {
        if (gameInput.IsJumpPressed() && isGrounded)
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}

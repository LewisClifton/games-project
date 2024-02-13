using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlidingMovement : MonoBehaviour
{
    [SerializeField] private float BaseSpeed;

    // Max thrust force
    [SerializeField] private float MaxThrustSpeed;

    // Minimum speed required for gliding thrust
    [SerializeField] private float MinThrustSpeed;

    // Camera, set to Main Camera
    [SerializeField] private Transform cameraTransform;

    // Used to determine thrust force
    [SerializeField] private float ThrustFactor;
    private float CurrentThrustSpeed;
    [SerializeField] private Rigidbody rigidBody;
    public bool isGrounded = false;
    public float groundedCheckDistance;
    private float bufferCheckDistance = 0.5f;
    
    // Layermask doesn't have to be only ground, it can be any layer you want the player to be able to stand on
    public LayerMask groundLayer;
    
    private void Start()
    {
        // Makes cursor invisible and locks it to centre of screen (esc during play mode to display it again)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FixedUpdate()
    {   
        checkGroundStatus();
        if (!isGrounded) {
            GlidingMovementF();
            ManageRotation();
        };
        if (isGrounded)
        {
            //Debug.Log("Grounded");
            //transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);

        }
    }

    //This function activates when isGrounded goes from False to True
    private void onLand()
    {
        //Makes player stand up
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
    }
    //Function used to check if the player is grounded
    private void checkGroundStatus()
    {
        groundedCheckDistance = (GetComponent<CapsuleCollider>().height / 2) + bufferCheckDistance; //Replace the capsule collider code with the height of the collider once the player model is changed.
        //Debug.Log(groundedCheckDistance);
        RaycastHit hit;
        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down;
        //out hit means we store the information in hit, and groundCheckDistance is how far the raycast goes
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
        //Debug.DrawRay(rayStart, rayDirection * groundedCheckDistance, isGrounded ? Color.green : Color.red);
    }
    //Calculates the forces that get applied during gliding
    private void GlidingMovementF()
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
        if (rigidBody.velocity.magnitude >= MinThrustSpeed)
        {
            //Applies glidingForce to the direction the player is pointing
            rigidBody.AddRelativeForce(glidingForce);
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
}

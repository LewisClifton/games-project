using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] private float BaseSpeed;

    // Max thrust force
    [SerializeField] private float MaxThrustSpeed;

    // Minimum speed required for gliding thrust
    [SerializeField] private float MinThrustSpeed;

    // Camera
    [SerializeField] private Transform cameraTransform;

    // Used to determine thrust force
    [SerializeField] private float ThrustFactor;
    private float CurrentThrustSpeed;
    [SerializeField] private Rigidbody rigidBody;
    public bool isGrounded = false;
    public float groundedCheckDistance;
    private float bufferCheckDistance = 0.5f;
    public LayerMask groundLayer;
    
    // Start is called before the first frame update
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
            GlidingMovement();
            ManageRotation();
        };
        Debug.Log(isGrounded);
    }
    // Update is called once per frame
    private void Update()
    {
        
    }

    //Function used to check if the player is grounded
    private void checkGroundStatus()
    {
        groundedCheckDistance = (GetComponent<CapsuleCollider>().height / 2) + bufferCheckDistance; //Replace the capsule collider code with the height of the collider once the player model is changed.
        //Debug.Log(groundedCheckDistance);
        RaycastHit hit;
        Vector3 rayStart = transform.position;
        Vector3 rayDirection = -transform.up;
        //out hit means we store the information in hit, and groundCheckDistance is how far the raycast goes
        if (Physics.Raycast(rayStart, rayDirection, out hit, groundedCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        Debug.DrawRay(rayStart, rayDirection * groundedCheckDistance, isGrounded ? Color.green : Color.red);
    }
    //Calculates the forces that get applied during gliding
    private void GlidingMovement()
    {
        float pitchInRads = transform.eulerAngles.x * Mathf.Deg2Rad;
        float mappedPitch=Mathf.Sin(pitchInRads) *ThrustFactor;
        Vector3 glidingForce = Vector3.forward * CurrentThrustSpeed;

        CurrentThrustSpeed += mappedPitch * Time.deltaTime;
        CurrentThrustSpeed = Mathf.Clamp(CurrentThrustSpeed, 0, MaxThrustSpeed);
        if (rigidBody.velocity.magnitude >= MinThrustSpeed)
        {
            rigidBody.AddRelativeForce(glidingForce);
        }
        else
        {
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

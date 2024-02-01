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
    // Start is called before the first frame update
    private void Start()
    {
        // Makes cursor invisible and locks it to centre of screen (esc during play mode to display it again)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FixedUpdate()
    {
        GlidingMovement();
        ManageRotation();
    }
    // Update is called once per frame
    private void Update()
    {
        
    }
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
    private void ManageRotation()
    {

        Quaternion targetRotation = Quaternion.Euler(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        transform.rotation = targetRotation;
    }
}

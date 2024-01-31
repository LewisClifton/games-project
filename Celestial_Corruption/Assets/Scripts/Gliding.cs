using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    [SerializeField] private float BaseSpeed;
    [SerializeField] private float MaxThrustSpeed;
    [SerializeField] private float MinThrustSpeed;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float ThrustFactor;
    private float CurrentThrustSpeed;
    private Rigidbody Rb;
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        GlidingMovement();
    }
    // Update is called once per frame
    private void Update()
    {
        ManageRotation();
    }
    private void GlidingMovement()
    {
        float pitchInRads = transform.eulerAngles.x * Mathf.Deg2Rad;
        float mappedPitch=Mathf.Sin(pitchInRads) *ThrustFactor;
        Vector3 glidingForce = Vector3.forward * CurrentThrustSpeed;

        CurrentThrustSpeed += mappedPitch * Time.deltaTime;
        CurrentThrustSpeed = Mathf.Clamp(CurrentThrustSpeed, 0, MaxThrustSpeed);
        
        Rb.AddRelativeForce(glidingForce);
    }
    private void ManageRotation()
    {

        Quaternion targetRotation = Quaternion.Euler(cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
        transform.rotation = targetRotation;
    }
}

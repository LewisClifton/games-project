using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TimeBody : MonoBehaviour
{
    public float TimeBeforeAffected; //The time after the object spawns until it will be affected by the timestop(for projectiles etc)
    private TimeManager timemanager;
    private Rigidbody rb;
    private Vector3 recordedVelocity;
    private float recordedMagnitude;
    private Quaternion recordedRotation;

    private float TimeBeforeAffectedTimer;
    private bool CanBeAffected;
    private bool IsStopped;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        timemanager = GameObject.FindGameObjectWithTag("TimeManager").GetComponent<TimeManager>(); // initial the timemanager

        if (timemanager == null)
        {
            Debug.LogError("TimeManager not found or not properly initialized!");  // see if we can get Timemanager
        }

        TimeBeforeAffectedTimer = TimeBeforeAffected;
    }

    // Update is called once per frame
    void Update()
    {
        TimeBeforeAffectedTimer -= Time.deltaTime; // minus 1 per second
        if (TimeBeforeAffectedTimer <= 0f)
        {
            CanBeAffected = true; // Will be affected by timestop
        }

        if (CanBeAffected && timemanager.TimeIsStopped && !IsStopped)
        {
            
                recordedVelocity = rb.velocity.normalized; //records direction of movement
                recordedMagnitude = rb.velocity.magnitude; // records magitude of movement
                recordedRotation = rb.rotation; //records rotation


                rb.velocity = Vector3.zero; //makes the rigidbody stop moving
                rb.angularVelocity = Vector3.zero; // stop angular velocity
                rb.isKinematic = true; //not affected by forces
                IsStopped = true; // prevents this from looping
            
        }

    }
    public void ContinueTime()
    {
        rb.isKinematic = false;
        IsStopped = false;
        rb.velocity = recordedVelocity * recordedMagnitude; //Adds back the recorded velocity when time continues
        rb.rotation = recordedRotation; //Restore rotation
    }
}

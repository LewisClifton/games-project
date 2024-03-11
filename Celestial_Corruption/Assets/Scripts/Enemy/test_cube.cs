using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_cube : MonoBehaviour
{
    [SerializeField] Transform Player;
    private bool shouldLookAt = true; // Flag to control whether to call LookAt()

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shouldLookAt)
        {
            //only target the player's x and z position
          Vector3 targetPostition = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);
          Vector3 direction = targetPostition - transform.position;
          Quaternion toRotation = Quaternion.LookRotation(direction);
          transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 2*Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.J)) 
        {
            StopRotation();
        }

        if (Input.GetKeyDown(KeyCode.K))  
        {
            ResumeRotation();
        }
    }

    // Method to stop rotation
    public void StopRotation()
    {
        shouldLookAt = false;
   
    }

    // Method to resume rotation
    public void ResumeRotation()
    {
        shouldLookAt = true;
    }
}



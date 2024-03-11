using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class test_cube : MonoBehaviour
{
    [SerializeField] Transform Player;


    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //only target the player's x and z position
        Vector3 targetPostition = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);
        Vector3 direction = targetPostition - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 2*Time.deltaTime);
       
    }

}


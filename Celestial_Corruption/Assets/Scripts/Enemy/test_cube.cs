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
    void Update()
    {
        //only target the player's x and z position
        Vector3 targetPostition = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);
        transform.LookAt(targetPostition);
       
    }

}


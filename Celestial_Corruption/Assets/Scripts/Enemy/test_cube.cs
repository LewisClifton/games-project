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
        transform.LookAt(Player);
       
    }

}

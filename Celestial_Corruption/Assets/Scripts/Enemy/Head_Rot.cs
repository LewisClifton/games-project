using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Heead_Rot : MonoBehaviour
{
    [SerializeField] Transform Player;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(Player);

    }

}


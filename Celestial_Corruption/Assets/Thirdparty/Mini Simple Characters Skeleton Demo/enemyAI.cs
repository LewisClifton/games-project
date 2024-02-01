using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float enemySpeed, dis;
    Vector3 StartPos;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        StartPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        dis = Vector3.Distance(transform.position, Player.position);
        if (dis <= 8f ) 
        {
            // chase
            chase();
        }
        if (dis > 8f)
        {
            // go back home
            goHome();
        }
    }

    void chase()
    {
        transform.LookAt(Player);
        transform.Translate(0,0,enemySpeed * Time.deltaTime);
    }

    void goHome()
    {
        transform.LookAt (StartPos);
        transform.position = Vector3.Lerp(transform.position,StartPos,0.002f);
    }
}

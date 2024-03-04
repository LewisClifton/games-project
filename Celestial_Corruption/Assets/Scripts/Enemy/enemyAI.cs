using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float enemySpeed;
    [SerializeField] float distance;
    private Rigidbody rb;
    Vector3 StartPos;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player").transform;
        StartPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, Player.position);
        if (distance <= 8f ) 
        {
            // chase
            chase();
        }
        if (distance > 8f)
        {
            // go back home
            goHome();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            rb.isKinematic = true; 
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            rb.isKinematic = false; 
        }
    }


    void chase()
    {
            Vector3 direction = (Player.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * enemySpeed * Time.deltaTime);
            transform.LookAt(Player);

    }

    void goHome()
    {
        transform.LookAt (StartPos);
        transform.position = Vector3.Lerp(transform.position,StartPos,0.002f);
    }
}

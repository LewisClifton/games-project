using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float enemySpeed;
    [SerializeField] float distance;
    [SerializeField] float chaseDistance; // this is the distance you should modify
    [SerializeField] float driftMin;
    [SerializeField] float driftMax;
    private Rigidbody rb;
    private Vector3 StartPos;

    void Start()
    {
        Player = GameObject.Find("Player").transform;
        StartPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, Player.position);
        if (distance <= chaseDistance)
        {
            // chase
            chase();
        }
        if (distance > chaseDistance)
        {
            // go back home
            goHome();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = true;
        }
        else if (collision.gameObject.CompareTag("Ground") && rb.velocity.y < 0)
        {
            // When it hit the ground it sets velocity to zero
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false;
        }
    }

    void chase()
    {
        Vector3 direction = (Player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * enemySpeed * Time.deltaTime);
        Vector3 targetPostition = new Vector3(Player.position.x, transform.position.y, Player.position.z);
        transform.LookAt(targetPostition);
    }

    void goHome()
    {
        Vector3 ReturnToStart = new Vector3(StartPos.x, transform.position.y, StartPos.z);
        transform.LookAt(ReturnToStart);
        transform.position += (ReturnToStart - transform.position) * Time.deltaTime * enemySpeed;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && rb.velocity.y < 0)
        {
            // Set thr draftspeed between driftMin and driftMax
            float driftSpeed = Random.Range(driftMin, driftMax);

            // Caculate the direction of reflection
            Vector3 direction = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);

            // Set velocity and avoid it's smaller than 0
            rb.velocity = direction * Mathf.Max(driftSpeed, 0f);
        }
    }
}


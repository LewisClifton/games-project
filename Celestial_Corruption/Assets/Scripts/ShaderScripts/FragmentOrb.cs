using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FragmentOrb : MonoBehaviour
{
    [SerializeField] private Renderer orbRenderer;
    private float radius;
    private Transform playerTransform;
    private Rigidbody playerRb;

    // Currently assuming the player has a Capsule Collider
    private CapsuleCollider playerCollider;
    private bool isFrozen = false;

    private void Start()
    {
        // Assuming the player has a Rigidbody component
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>();
        radius = transform.lossyScale.x * playerCollider.radius;
    }

    private void FixedUpdate()
    {
        // Calculate the player's distance from the orb
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        float threshold = 10f;


        if (distance < 8)
            {
                 SceneManager.LoadScene(2);
            }

        // If the player is within a threshold distance from the orb, send the player's position to the shader
        if (distance < threshold)
        {
            // Project player position onto the collider of the orb
            Vector3 playerPositionProjection = (playerTransform.position - transform.position).normalized * radius;
            orbRenderer.material.SetVector("_PlayerPosition", playerPositionProjection);



        } else {
            orbRenderer.material.SetVector("_PlayerPosition", new Vector3(0, 0, 0));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 worldCenter = transform.TransformPoint(playerCollider.center);

            // For a Capsule Collider, we use radius and height for its size
            // Transforming radius to world space (assuming uniform scaling for simplicity)
            float worldRadius = transform.lossyScale.x * playerCollider.radius;
            float worldHeight = transform.lossyScale.y * playerCollider.height;

            // Debug.Log("Collider center: " + worldCenter);

            // Send data to the shader
            orbRenderer.material.SetVector("_ColliderCenter", worldCenter);
            orbRenderer.material.SetFloat("_ColliderRadius", worldRadius);
            orbRenderer.material.SetFloat("_ColliderHeight", worldHeight);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make the player immovable
            if (playerRb != null && !isFrozen)
            {
                Debug.Log("Player collided with the orb");
                StartCoroutine(freezeCoroutine(3f));
                Debug.Log("Player is frozen for 3 seconds");
            }
        }
    }

    IEnumerator freezeCoroutine(float seconds)
    {
        isFrozen = true;
        RigidbodyConstraints constraints = playerRb.constraints;
        playerRb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(seconds);
        playerRb.constraints = constraints;
        isFrozen = false;
        SceneManager.LoadScene(2);
    }
}

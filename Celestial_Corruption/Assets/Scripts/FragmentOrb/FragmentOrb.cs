using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentOrb : MonoBehaviour
{
    [SerializeField] private Renderer orbRenderer;
    private Rigidbody playerRb;
    private Collider playerCollider;
    private bool isFrozen = false;

    private void Start()
    {
        // Assuming the player has a Rigidbody component
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with the orb");
            orbRenderer.material.SetVector("_EnteringObjectPosition", playerCollider.size + playerCollider.center);
            orbRenderer.material.SetFloat("_ColliderCenter", playerCollider.center);
            orbRenderer.material.SetFloat("_ColliderSize", playerCollider.size);
            // orbRenderer.material.SetVector("_EnteringObjectPosition", other.bounds.center);
        }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     // Check if the collision is with the player
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         // Make the player immovable
    //         if (playerRb != null && !isFrozen)
    //         {
    //             Debug.Log("Player collided with the orb");
    //             StartCoroutine(freezeCoroutine(3f));
    //             Debug.Log("Player is frozen for 3 seconds");
    //         }
    //     }
    // }

    IEnumerator freezeCoroutine(float seconds)
    {
        isFrozen = true;
        RigidbodyConstraints constraints = playerRb.constraints;
        playerRb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(seconds);
        playerRb.constraints = constraints;
        isFrozen = false;
    }
}

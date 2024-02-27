using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentOrb : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make the collided object immovable
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentOrb : MonoBehaviour
{
    private Rigidbody playerRb;
    private bool isFrozen = false;

    private void Start()
    {
        // Assuming the player has a Rigidbody component
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make the player immovable
            if (playerRb != null)
            {
                // playerRb.constraints = RigidbodyConstraints.FreezeAll;
                StartCoroutine(freezeCoroutine(3f));
            }
        }
    }

    IEnumerator freezeCoroutine(float seconds)
    {
        isFrozen = true;
        playerRb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(seconds);
        playerRb.constraints = RigidbodyConstraints.None;
        isFrozen = false;
    }
}

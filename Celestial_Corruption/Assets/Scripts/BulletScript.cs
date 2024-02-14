using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Destory the bullet if it touches the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            Destroy(gameObject);
        }
    }
}

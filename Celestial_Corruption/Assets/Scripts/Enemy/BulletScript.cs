using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public PlayerHealth playerHealth;
    [SerializeField] private int damage = 10;
    private void Start()
    {
        // Get the PlayerHealth component and assign it to the PlayerHealth variable
        // Need this line to make the damage work
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();

        // this line here is just to change the damage figure
        damage = 5;

    }

    private void Update()
    {
        Debug.Log(damage);
    }

    // Destory the bullet if it touches the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            playerHealth.TakeDamage(damage);
            
            Destroy(gameObject);
        }
    }
}

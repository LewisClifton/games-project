using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Change damage figure in the prefab
    public PlayerHealth playerHealth;
    [SerializeField] int damage;
    public ParticleSystem deathParticles;
    private float initializationTime;
    float timeSinceInitialization;
    private void Start()
    {
        // Get the PlayerHealth component and assign it to the PlayerHealth variable
        // Need this line to make the damage work
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        initializationTime = Time.timeSinceLevelLoad;

        // this line here is just to change the damage figure
        //damage = 5;

    }

    private void Update()
    {
        //calculating how long this object has been instantiated
        timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
    }

    // Destory the bullet if it touches the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Banana Man"))
        {
            playerHealth.TakeDamage(damage);

            Destroy(0f);

        } else if(timeSinceInitialization > 0.1) //colsion with other objects after it has left the collision range with itself
        {
            Destroy(0.2f);
        }
    }

    public void Destroy(float timer)
    {
        //create a death particle
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        //destroy this spehere
        Destroy(gameObject, timer);
    }
}

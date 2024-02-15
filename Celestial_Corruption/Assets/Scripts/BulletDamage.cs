using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damage = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Trigger or not");
        }
    }
}

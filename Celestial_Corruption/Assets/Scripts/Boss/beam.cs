using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beam : MonoBehaviour
{
    [SerializeField] PlayerHealth PlayerHealth;
    [SerializeField] float damage;
    // Start is called before the first frame update
    void Start()
    {
        // Get the PlayerHealth component and assign it to the PlayerHealth variable
        PlayerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            PlayerHealth.TakeDamage(damage);

        }
    }
}

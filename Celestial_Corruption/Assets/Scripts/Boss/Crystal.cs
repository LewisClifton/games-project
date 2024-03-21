using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
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

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            PlayerHealth.TakeDamage(damage);
            Debug.Log("Crystal damage");
            Destroy(gameObject);
        }
    }
}

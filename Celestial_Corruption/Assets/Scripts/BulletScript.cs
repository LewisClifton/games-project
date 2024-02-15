using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int damage = 10;
    private void Start()
    {
        // 获取 PlayerHealth 组件并赋值给 PlayerHealth 变量
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        
    }

    // Destory the bullet if it touches the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Player"))
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Trigger or not");
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform Player;

    [SerializeField] Transform enemy;

    [SerializeField] private float timer = 5;

    [SerializeField] float dis;

    private float bulletTime;

    public GameObject enemyBullet;

    public Transform spawnPoint;

    public float enemySpeed;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        dis = Vector3.Distance(transform.position, Player.position);

        if (dis <= 60f)
        {
            ShootAtPlayer();
        }
        //ShootAtPlayer();
    }

    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if (bulletTime > 0) return;

        bulletTime = timer;

        GameObject bulletObj = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();
        //bulletRig.AddForce(bulletRig.transform.forward * enemySpeed);

        // Calculate the direction of the bullet's movement, from the bullet's position to the player's position
        Vector3 moveDirection = (Player.position - spawnPoint.position).normalized;

        // Face to the Player
        Quaternion rotation = Quaternion.LookRotation(moveDirection);
        bulletObj.transform.rotation = rotation;

        // Add initial velocity to the bullet to make it shoot in a straight line
        bulletRig.velocity = moveDirection * enemySpeed;

        Destroy(bulletObj, 5f);
    }
}
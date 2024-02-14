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
        ShootAtPlayer();
    }

    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if (bulletTime > 0) return;

        bulletTime = timer;

        GameObject bulletObj = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();
        //bulletRig.AddForce(bulletRig.transform.forward * enemySpeed);
        
        // 计算子弹移动方向，从子弹位置指向玩家位置
        Vector3 moveDirection = (Player.position - spawnPoint.position).normalized;

        // 朝向玩家
        Quaternion rotation = Quaternion.LookRotation(moveDirection);
        bulletObj.transform.rotation = rotation;

        // 给子弹添加初速度使其直线发射
        bulletRig.velocity = moveDirection * enemySpeed;

        Destroy(bulletObj, 5f);
    }
}
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
        
        // �����ӵ��ƶ����򣬴��ӵ�λ��ָ�����λ��
        Vector3 moveDirection = (Player.position - spawnPoint.position).normalized;

        // �������
        Quaternion rotation = Quaternion.LookRotation(moveDirection);
        bulletObj.transform.rotation = rotation;

        // ���ӵ���ӳ��ٶ�ʹ��ֱ�߷���
        bulletRig.velocity = moveDirection * enemySpeed;

        Destroy(bulletObj, 5f);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] Transform Player;
    [SerializeField] float laserSpeed;
    private GameObject currentLaser;
    // Start is called before the first frame update
    void Start()
    {
        currentLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        currentLaser.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // 检查玩家是否存在
        if (Player != null)
        {
            // 让激光的方向朝向玩家
            Vector3 direction = Player.position - transform.position;
            currentLaser.transform.up = direction.normalized;

            // 计算玩家与boss之间的距离，并将激光拉长或缩短
            float distance = Vector3.Distance(Player.position, transform.position);
            currentLaser.transform.localScale = new Vector3(1, distance, 1);

            // 激光沿着方向移动
            currentLaser.transform.position += currentLaser.transform.up * laserSpeed * Time.deltaTime;
        }
        else
        {
            // 玩家不存在时，销毁激光
            Destroy(currentLaser);
        }
    }
}

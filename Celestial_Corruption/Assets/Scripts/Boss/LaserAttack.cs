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
        // �������Ƿ����
        if (Player != null)
        {
            // �ü���ķ��������
            Vector3 direction = Player.position - transform.position;
            currentLaser.transform.up = direction.normalized;

            // ���������boss֮��ľ��룬������������������
            float distance = Vector3.Distance(Player.position, transform.position);
            currentLaser.transform.localScale = new Vector3(1, distance, 1);

            // �������ŷ����ƶ�
            currentLaser.transform.position += currentLaser.transform.up * laserSpeed * Time.deltaTime;
        }
        else
        {
            // ��Ҳ�����ʱ�����ټ���
            Destroy(currentLaser);
        }
    }
}

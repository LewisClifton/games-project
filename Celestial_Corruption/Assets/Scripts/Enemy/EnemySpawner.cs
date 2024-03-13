using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // [SerializeField] private GameObject enemySpawner;
    [SerializeField] private float positiveOffset = 5f;
    [SerializeField] private float triangleLength = 10f;
    [SerializeField] private float triangleWidth = 5f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float enemyWidth = 2f;
    [SerializeField] private float enemyLength = 2f;
    [SerializeField] private float spawnRate = 1f;
    [SerializeField] private bool visualiseSpawnArea = true;
    private int maxEnemies = 10;
    private int currentEnemies = 0;

    private void Awake()
    {

    }

    private void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private void Update()
    {

    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies)
            {
                Vector3 spawnPosition = CalculateSpawnPosition();
                Vector3 boxSize = new Vector3(enemyWidth / 2, 1, enemyLength / 2);

                if (Physics.BoxCast(spawnPosition, boxSize / 2, Vector3.down, out RaycastHit hit, Quaternion.identity, Mathf.Infinity))
                {
                    Instantiate(enemyPrefab, hit.point, Quaternion.identity);
                }

                Debug.DrawRay(spawnPosition, Vector3.down, Color.red, 10f);
                Debug.Log("Spawn position: " + spawnPosition);
            }
            yield return new WaitForSeconds(1 / spawnRate);
        }
    }

    private Vector3 CalculateSpawnPosition()
    {
        float x = Random.Range(-triangleWidth / 2, triangleWidth / 2);
        float norm = (triangleLength + triangleWidth/2) / 2;
        float ratio = triangleLength / (triangleWidth / 2);
        float ratio_norm = ratio / norm;
        float y = Random.Range(1-((1-Mathf.Abs(x))/ratio)*norm, triangleLength);

        // Transform random triangle position with the player rotation
        float w_x = this.transform.forward.x * y + this.transform.position.x;
        float w_y = this.transform.forward.z * x + this.transform.position.z;
        return new Vector3(w_x, 0, w_y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta; // Set the color of the triangle

        // Calculate the three vertices of the triangle
        Vector3 vertex1 = transform.position + transform.forward * positiveOffset; // Tip of the triangle
        Vector3 widthDirection = Quaternion.Euler(0, -90, 0) * transform.forward; // Perpendicular to forward direction
        Vector3 vertex2 = transform.position + transform.forward * triangleLength + widthDirection * (triangleWidth / 2); // Right base corner
        Vector3 vertex3 = transform.position + transform.forward * triangleLength - widthDirection * (triangleWidth / 2); // Left base corner

        // Draw lines between the vertices
        Gizmos.DrawLine(vertex1, vertex2);
        Gizmos.DrawLine(vertex2, vertex3);
        Gizmos.DrawLine(vertex3, vertex1);
    }
}

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
        if (visualiseSpawnArea)
        {
            OnDrawGizmos();
        }
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

                // Make a BoxCast to check if the spawn position is clear from top to bottom
                Vector3 boxSize = new Vector3(enemyWidth / 2, 1, enemyLength / 2);
                RaycastHit[] hits = Physics.BoxCastAll(spawnPosition, boxSize, Vector3.up, Quaternion.identity, 1);

                Debug.DrawRay(spawnPosition, Vector3.up, Color.red, 1f);

                if (Physics.BoxCast(boxCastStartPoint, boxSize / 2, Vector3.down, out RaycastHit hit, Quaternion.identity, Mathf.Infinity))
                {
                    Intantiate(enemyPrefab, hit.point, Quaternion.identity);
                }

                // Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                // currentEnemies++;
            }
            yield return new WaitForSeconds(1 / spawnRate);
        }
    }

    private Vector3 CalculateSpawnPosition()
    {
        float randomWidth = Random.Range(0f, triangleWidth);
        float ratio = randomWidth / triangleWidth;
        float randomLength = Random.Range(positiveOffset, triangleLength * (1 - ratio) + positiveOffset);

        // float randomWidth = Random.Range(0f, triangleWidth);
        // float ratio = randomWidth / triangleWidth;
        // float randomLength = Random.Range(0f, triangleLength * (1 - ratio));

        // // Assuming the triangle's tip is at the GameObject's position and it extends forward
        Vector3 widthDirection = Quaternion.Euler(0, -90, 0) * transform.forward; // Perpendicular to forward direction
        Vector3 spawnDirection = transform.forward * randomLength + widthDirection * (randomWidth - triangleWidth / 2);

        return transform.position + spawnDirection;
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

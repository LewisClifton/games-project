using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalAttack : MonoBehaviour
{
    [SerializeField] GameObject CrystalPrefab;
    [SerializeField] GameObject boss;
    [SerializeField] float Skyhigh; // the height of the crystal
    [SerializeField] int SpawnPoints; // number of crystals
    [SerializeField] float minSpawnRadius; // Minimum crystal distance from boss
    [SerializeField] float maxSpawnRadius; // Maximum crystal distance from boss
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            SpawnCrystals();
        }
    }

    // Random Generate crystals
    public void SpawnCrystals()
    {
        for (int i = 0; i < SpawnPoints; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere; // Get a random direction
            randomDirection.y = 0; // Ensure it's on the same plane as boss (2D)
            randomDirection.Normalize(); // Normalize it to have a magnitude of 1
            float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius); // Random distance within range
            Vector3 spawnPosition = boss.transform.position + randomDirection * randomDistance; // Calculate spawn position
            spawnPosition.y = Skyhigh; // Set the height of the crystal
            Instantiate(CrystalPrefab, spawnPosition, Quaternion.identity); // Instantiate crystal at spawn position
        }
    }
}

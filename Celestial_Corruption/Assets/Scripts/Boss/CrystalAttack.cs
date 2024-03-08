using UnityEngine;

public class CrystalAttack : MonoBehaviour
{
    [SerializeField] GameObject CrystalPrefab;
    [SerializeField] GameObject boss;
    [SerializeField] float Skyhigh; // the height of the crystal
    [SerializeField] int minCrystalNumber; // minimum number of crystals to spawn
    [SerializeField] int maxCrystalNumber; // maximum number of crystals to spawn
    [SerializeField] float minSpawnRadius; // Minimum crystal distance from boss
    [SerializeField] float maxSpawnRadius; // Maximum crystal distance from boss
    [SerializeField] float spawnCooldown; // Cooldown time between each crystal spawn
    [SerializeField] float crystalDestoryTime; // The time for crystal to destory
    private float lastSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time; // Set last spawn time to current time to ensure immediate spawning
    }

    // Update is called once per frame
    void Update()
    {
        // Check if enough time has passed since the last spawn
        if (Time.time - lastSpawnTime >= spawnCooldown)
        {
            SpawnCrystals();
            lastSpawnTime = Time.time; // Update last spawn time
        }
    }

    // Random Generate crystals
    public void SpawnCrystals()
    {
        int crystalNumber = Random.Range(minCrystalNumber, maxCrystalNumber + 1); // Randomize the number of crystals to spawn within the range

        for (int i = 0; i < crystalNumber; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere; // Get a random direction
            randomDirection.y = 0; // Ensure it's on the same plane as boss (2D)
            randomDirection.Normalize(); // Normalize it to have a magnitude of 1
            float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius); // Random distance within range
            Vector3 spawnPosition = boss.transform.position + randomDirection * randomDistance; // Calculate spawn position
            spawnPosition.y = Skyhigh; // Set the height of the crystal
            GameObject crystal = Instantiate(CrystalPrefab, spawnPosition, Quaternion.identity); // Instantiate crystal at spawn position
            Destroy(crystal, crystalDestoryTime); // Destroy the crystal after 5 seconds
        }
    }
}



using UnityEngine;

public class CrystalAttack : MonoBehaviour
{
    [SerializeField] GameObject CrystalPrefab;
    [SerializeField] GameObject MagicCirclePrefab; // Prefab for circle
    [SerializeField] GameObject boss;
    [SerializeField] float height; // the height of the crystal
    [SerializeField] float circleY; // this is to ensure the circle Y location in case some problems
    [SerializeField] float crystaldistance; // the distance use to avoid crystals collision problem
    [SerializeField] int minCrystalNumber; // minimum number of crystals to spawn
    [SerializeField] int maxCrystalNumber; // maximum number of crystals to spawn
    [SerializeField] float minSpawnRadius; // Minimum crystal distance from boss
    [SerializeField] float maxSpawnRadius; // Maximum crystal distance from boss
    [SerializeField] float spawnCooldown; // Cooldown time between each crystal spawn
    [SerializeField] float crystalDestoryTime; // The time for crystal to destroy
    [SerializeField] float fallSpeed; // Fall speed of the crystals
    [SerializeField] float hovertime; // The time for crystal to hover before falling
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
            spawnPosition.y = height; // Set the height of the crystal

            // Check if there's any crystal too close to the spawn position
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, crystaldistance); // Adjust the radius as needed
            bool hasCloseCrystal = false;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Crystal"))
                {
                    hasCloseCrystal = true;
                    break;
                }
            }

            // If there's a crystal too close, skip this iteration
            if (hasCloseCrystal)
            {
                continue;
            }

            // Instantiate crystal at spawn position
            GameObject crystal = Instantiate(CrystalPrefab, spawnPosition, Quaternion.identity) as GameObject;
            crystal.SetActive(true); // Ensure the instance is set to active

            // Instantiate circle
            GameObject circle = Instantiate(MagicCirclePrefab, new Vector3(crystal.transform.position.x, circleY, crystal.transform.position.z),Quaternion.identity);
            circle.SetActive(true);
            // here add the code to disable the gravity and enable it after hovertime
            HoverCrystal(crystal);
            Destroy(crystal, crystalDestoryTime); // Destroy the crystal after a certain time
            Destroy(circle, crystalDestoryTime); // Destory the crystal after a certain time
        }
    }

    public void HoverCrystal(GameObject crystal)
    {
        Rigidbody crystalRigidbody = crystal.GetComponent<Rigidbody>();
        if (crystalRigidbody != null)
        {
            crystalRigidbody.useGravity = false; //Disable gravity for the crystal
            // Invoke method to restore gravity after hover time
            Invoke("ApplyFallSpeed", hovertime);
        }
    }

    public void ApplyFallSpeed()
    {
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal"); // Find all crystals in the scene
        foreach (GameObject crystal in crystals)
        {
            // Add downward velocity to the crystal
            Rigidbody crystalRigidbody = crystal.GetComponent<Rigidbody>();
            if (crystalRigidbody != null)
            {
                crystalRigidbody.useGravity = true;
                crystalRigidbody.velocity = new Vector3(0, -fallSpeed, 0); // Apply falling velocity
            }
        }
    }
}




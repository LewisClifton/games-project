using sc.terrain.vegetationspawner;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class Boss : MonoBehaviour
{
    public int maxHealth = 40;
    private int currentHealth;
    private int currentPhase = 1;

    [SerializeField] GameObject playerObject;

    public Transform player;
    public Transform AttackPos;
    public Transform FootPos;

    public GameObject curveAttackObj;
    public GameObject stompAttackObj;
    public GameObject boxAttackObj;
    public GameObject spearAttackObj;

    private float invincibilityTime;
    public float maxInvincibilityTime;
    public float deathDistance;
    public int enemyXP;
    public float dis;


    private GameObject lineRendererObject;
    private CapsuleCollider playerCollider;
    private MeshCollider lineCollider;
    private Vector3 playerPos;
    private Animator animator;
    private bool playerCD = false;
    PlayerHealth playerHealth;

    [Header("SkyCrystalAttack")]

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

    [Header("GroundCrystalAttack")]

    [SerializeField] GameObject bossprefab;
    [SerializeField] GameObject trapPrefab; // the circle show the trap
    [SerializeField] GameObject beamPrefab; // the beam gonna do damage
    [SerializeField] float maxX;
    [SerializeField] float maxZ;
    [SerializeField] float spawndistance; // distance between each of them
    [SerializeField] int number; // how many of these
    [SerializeField] float trapDuration; // the duration of the trap
    [SerializeField] float beamDuration; // the duration of the beam
    [SerializeField] float beamHeight; // the height of the beam
    private List<Vector3> trapPositions = new List<Vector3>(); // Keep track of trap positions


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        playerPos = player.position;
        playerCollider = player.GetComponent<CapsuleCollider>();
        playerHealth = player.GetComponent<PlayerHealth>();
        InvokeRepeating("PlayerPos", 0f, 5f);
        InvokeRepeating("RandomAttack", 0.2f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        HealthCheck(healthPercentage);
        if (lineRendererObject != null)
        {
            if (!playerCD && (playerCollider && lineCollider.bounds.Intersects(playerCollider.bounds)))
            {
                Debug.Log("collision");
                playerHealth.TakeDamage(10);
                playerCD = true;
                Invoke("CDOver", 1f);
            }
        }
        if (invincibilityTime != 0)
        {
            Debug.Log(invincibilityTime);
            invincibilityTime -= Time.deltaTime;
            if (invincibilityTime < 0)
            {
                invincibilityTime = 0;
            }
        }
        dis = Vector3.Distance(AttackPos.position, player.position);
        if (playerObject.layer == LayerMask.NameToLayer("Dashing"))
        {
            if (dis <= deathDistance)
            {
                takeDamage();
            }
        }
    }
    void CDOver()
    {
        playerCD = false;
    }

    private void takeDamage()
    {
        if (invincibilityTime == 0)
        {
            currentHealth -= 1;
            invincibilityTime = maxInvincibilityTime;
            Debug.Log(currentHealth);
        }

        if (currentHealth == 0)
        {

            ScoreManager.instance.AddScore(enemyXP);
            Debug.Log("Died!");
            Destroy(gameObject);
        }
    }

    // Here need to modify to achieve random attack
    void HealthCheck(float healthPercent)
    {
        if (healthPercent <= 0.8f && healthPercent > 0.6f)
        {
            currentPhase = 2;
        }
        else if(healthPercent <= 0.6f && healthPercent > 0.4f)
        {
            currentPhase = 3;
        }
        else if(healthPercent <= 0.4f && healthPercent > 0.2f)
        {
            currentPhase = 4;
        }
        else if (healthPercent <= 0.2f && healthPercent > 0f)
        {
            currentPhase = 5;
        }
    }

    void RandomAttack()
    {
        int randomAttack = Random.Range(1, 6);
        switch (randomAttack)
        {
            case 1:
                animator.SetBool("IsCrouching", true);//needs unique animation
                Invoke("BeamAttack", 1f);
                break;
            case 2:
                animator.SetBool("IsAiming", true);//needs unique animation
                Invoke("CurvedProjectileAttack", 1f);
                break;
            case 3:
                animator.SetBool("IsStomping", true);
                Invoke("StompAttack", 1.8f);
                Invoke("StopStomping", 1f);
                break;
            case 4:
                animator.SetBool("IsSummoning", true);
                Invoke("SkyCrystalAttack", 1f);
                break;
            case 5:
                animator.SetBool("IsSummoning", true);
                Invoke("GroundCrystalAttack", 1f);
                break;
            case 6:
                UniqueAttack();
                break;
        }
    }

    void UniqueAttack()
    {
        switch(currentPhase)
        {
            case 1:
                animator.SetBool("IsStomping", true);
                Debug.Log("un stomp");
                Invoke("StompAttack", 1.8f);//animation is done just needs unique attack
                Invoke("StopStomping", 1f);
                break;
            case 2:
                animator.SetBool("IsSpinning", true);
                Debug.Log("un spin");
                Invoke("SpinAttack", 1f);//animation is done just needs unique attack
                break;
            case 3:
                animator.SetBool("IsSummoning", true);
                Debug.Log("un crystalAttack");
                Invoke("SkyCrystalAttack", 1f);//animation is done just needs unique attack
                break;
            case 4:
                animator.SetBool("IsSummoning", true);
                Debug.Log("un ground");
                Invoke("GroundCrystalAttack", 1f);//animation is done just needs unique attack
                break;
        }
    }

    void PlayerPos()
    {
        playerPos = player.position;
    }

    void BeamAttack()
    {
        lineRendererObject = new GameObject("LineRendererObject");
        LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 0.2f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f)},
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f)}
            );
        lineRenderer.colorGradient = gradient;
        lineRenderer.transform.parent = transform;
        lineRenderer.SetPosition(0, AttackPos.position);
        lineRenderer.SetPosition(1, playerPos);

        
        lineCollider = lineRenderer.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        lineCollider.sharedMesh = mesh;

        lineCollider.transform.position = lineRenderer.transform.position;
        lineCollider.transform.localScale = lineRenderer.transform.localScale; 

        Destroy(lineRendererObject, 2f);

        Invoke("StopCrouching", 1f);
    } 
    void StopCrouching()
    {
        animator.SetBool("IsCrouching", false);
    }

    public void CurvedProjectileAttack()
    {
        StartCoroutine(SpawnCurvedProjectiles());
        Invoke("StopAiming", 1f);
    }

    IEnumerator SpawnCurvedProjectiles()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject projectile = Instantiate(curveAttackObj, AttackPos.position + (Random.insideUnitSphere * 20f), Quaternion.identity);
            CurvedProjectile curvedScript = projectile.GetComponent<CurvedProjectile>();
            curvedScript.player = player;
            curvedScript.playerHealth = playerHealth;

            yield return new WaitForSeconds(0.2f);
        }
    }
    void StopAiming()
    {
        animator.SetBool("IsAiming", false);
    }

    void StompAttack()
    {
        Debug.Log("STOOOOMP");
        Vector3 footPos = new Vector3(FootPos.position.x, FootPos.position.y - 0.7f, FootPos.position.z);
        GameObject wave = Instantiate(stompAttackObj, footPos, Quaternion.identity);
        wave.transform.Rotate(90f, 0f, 0f);
        StompWave stompScript = wave.GetComponent<StompWave>();
        stompScript.player = player;
    }
    void StopStomping()
    {
        animator.SetBool("IsStomping", false);
    }

    void SpinAttack()
    {
        Debug.Log("SPiiiiiiiinnnnnn");
        StartCoroutine(SpawnCurvedProjectiles());//DELETE THIS LINE AFTER MVP
        Invoke("StopSpinning", 1f);
    }
    void StopSpinning()
    {
        animator.SetBool("IsSpinning", false);
    }

    // this is where implement the crystal attack script
    void SkyCrystalAttack()
    {
        SpawnCrystals();
        Debug.Log("From the sky");
        Invoke("StopSummoning", 1f);

    }

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
            GameObject circle = Instantiate(MagicCirclePrefab, new Vector3(crystal.transform.position.x, circleY, crystal.transform.position.z), Quaternion.identity);
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
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal");
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

    // this is where implement the beam atatck script
    void GroundCrystalAttack()
    {
        Generatetrap();
        Debug.Log("From the ground");
        Invoke("StopSummoning", 1f);
    }

    public void Generatetrap()
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 trapPosition = GenerateRandomTrapPosition();
            GameObject newTrap = Instantiate(trapPrefab, trapPosition, Quaternion.identity) as GameObject;
            newTrap.SetActive(true);
            Destroy(newTrap, trapDuration); // Destroy trap after trapDuration

            // Generate beamObject after trap disappears
            StartCoroutine(GenerateBeamAfterTrapDisappears(trapPosition));
        }
    }

    Vector3 GenerateRandomTrapPosition()
    {
        float bossX = boss.transform.position.x;
        float bossZ = boss.transform.position.z;
        Vector3 trapPosition = Vector3.zero;
        bool positionFound = false;

        while (!positionFound)
        {
            trapPosition = new Vector3(Random.Range(bossX - maxX, bossX + maxX), 0f, Random.Range(bossZ - maxZ, bossZ + maxZ));
            positionFound = true;

            foreach (Vector3 existingTrapPosition in trapPositions)
            {
                if (Vector3.Distance(trapPosition, existingTrapPosition) < spawndistance)
                {
                    positionFound = false;
                    break;
                }
            }
        }

        trapPositions.Add(trapPosition);
        return trapPosition;
    }

    IEnumerator GenerateBeamAfterTrapDisappears(Vector3 trapPosition)
    {
        yield return new WaitForSeconds(trapDuration);

        // Generate beamObject at the position of newTrap
        GameObject newBeam = Instantiate(beamPrefab, trapPosition, Quaternion.identity) as GameObject;
        newBeam.SetActive(true);
        // Scale the beam to match beamHeight
        newBeam.transform.localScale = new Vector3(newBeam.transform.localScale.x, beamHeight, newBeam.transform.localScale.z);
        Destroy(newBeam, beamDuration); // Destroy beamObject after beamDuration
    }


    void StopSummoning()
    {
        animator.SetBool("IsSummoning", false);
    }
}

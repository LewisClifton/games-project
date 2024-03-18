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

    void HealthCheck(float healthPercent)
    {
        if (healthPercent <= 0.75f && healthPercent > 0.5f)
        {
            currentPhase = 2;
        }
        else if(healthPercent <= 0.5f && healthPercent > 0.25f)
        {
            currentPhase = 3;
        }
        else if(healthPercent <= 0.25f)
        {
            currentPhase = 4;
        }
    }

    void RandomAttack()
    {
        int randomAttack = Random.Range(1, 4);
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
                Invoke("StompAttack", 1.8f);//animation is done just needs unique attack
                Invoke("StopStomping", 1f);
                break;
            case 2:
                animator.SetBool("IsSpinning", true);
                Invoke("SpinAttack", 1f);//animation is done just needs unique attack
                break;
            case 3:
                animator.SetBool("IsSummoning", true);
                Invoke("SkyCrystalAttack", 1f);//animation is done just needs unique attack
                break;
            case 4:
                animator.SetBool("IsSummoning", true);
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

    void SkyCrystalAttack()  // here is where add the code 
    {
        CrystalAttack crystalAttack = GetComponent<CrystalAttack>();
        crystalAttack.SpawnCrystals();
        crystalAttack.ApplyFallSpeed();
        Debug.Log("From the sky");
        Invoke("StopSummoning", 1f);

    }

    void GroundCrystalAttack()
    {
        BeamAttack beamAttack = GetComponent<BeamAttack>();
        beamAttack.Generatetrap();
        Debug.Log("From the ground");
        Invoke("StopSummoning", 1f);
    }
    void StopSummoning()
    {
        animator.SetBool("IsSummoning", false);
    }
}

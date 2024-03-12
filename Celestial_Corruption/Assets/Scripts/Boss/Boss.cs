using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;
    private int currentPhase = 1;

    public Transform player;

    public GameObject beamAttackObj;
    public GameObject stompAttackObj;
    public GameObject boxAttackObj;
    public GameObject spearAttackObj;

    private GameObject lineRendererObject;
    private CapsuleCollider playerCollider;
    private MeshCollider lineCollider;
    private Vector3 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        playerPos = player.position;
        playerCollider = player.GetComponent<CapsuleCollider>();
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
            if (playerCollider && lineCollider.bounds.Intersects(playerCollider.bounds))
            {
                Debug.Log("collision");
            }
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
                BeamAttack();
                break;
            case 2:
                CurvedProjectileAttack();
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
                StompAttack();
                break;
            case 2:
                SpinAttack();
                break;
            case 3:
                SkyCrystalAttack();
                break;
            case 4:
                GroundCrystalAttack();
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
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, playerPos);

        
        lineCollider = lineRenderer.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        lineCollider.sharedMesh = mesh;

        lineCollider.transform.position = lineRenderer.transform.position;
        lineCollider.transform.localScale = lineRenderer.transform.localScale; 

        Destroy(lineRendererObject, 2f);

    }

    void CurvedProjectileAttack()
    {
        Debug.Log("Projetileeeeeeeeeee_____eeee");
    }

    void StompAttack()
    {
        Debug.Log("STOOOOMP");
    }

    void SpinAttack()
    {
        Debug.Log("SPiiiiiiiinnnnnn");
    }

    void SkyCrystalAttack()
    {
        Debug.Log("From the sky");

    }

    void GroundCrystalAttack()
    {
        Debug.Log("From the ground");
    }
}

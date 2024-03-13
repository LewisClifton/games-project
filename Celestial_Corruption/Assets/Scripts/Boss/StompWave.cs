using AssetUsageDetectorNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StompWave : MonoBehaviour
{
    [SerializeField] int pointsCount;
    [SerializeField] float maxRadius;
    [SerializeField] float speed;
    [SerializeField] float startWidth;
    public Transform player;

    private LineRenderer lineRenderer;
    MeshCollider lineCollider;
    private bool damageTaken = false;

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = pointsCount + 1;

        StartCoroutine(Blast());
    }
    private IEnumerator Blast()
    {
        float currentRadius = 0f;

        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            yield return null;
        }
        Destroy(gameObject);
    }


    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;

        for (int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;

            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);

        GameObject.Destroy(lineCollider);
        lineCollider = lineRenderer.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();

        lineRenderer.BakeMesh(mesh);
        lineCollider.sharedMesh = mesh;
        lineCollider.convex = true;
        lineCollider.isTrigger = true;

        lineCollider.transform.position = lineRenderer.transform.position;
        lineCollider.transform.localScale = lineRenderer.transform.localScale;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!damageTaken && (other.gameObject == player.gameObject))
        {
            Debug.Log("Hit");
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(10);
            damageTaken = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedProjectile : MonoBehaviour
{
    public Transform player;
    public float curveSpeed = 1f;
    public ParticleSystem deathParticles;

    private Vector3[] curvePoints;
    private float t = 0f;
    public PlayerHealth playerHealth;

    void Start()
    {
        Destroy(gameObject, 3.5f);
        Vector3 startPoint = transform.position;
        Vector3 endPoint = player.position + (Random.insideUnitSphere * 5f);
        Vector3 startTangent = startPoint + Random.insideUnitSphere * 10f;
        Vector3 endTangent = endPoint + Random.insideUnitSphere * 8f;

        curvePoints = new Vector3[] { startPoint, startTangent, endTangent, endPoint };
    }

    void Update()
    {
        t += Time.deltaTime * curveSpeed;
        t = Mathf.Clamp01(t);
        Vector3 newPosition = BezierCurve(curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], t);

        transform.position = newPosition;

        if (t >= 1f)
            Destroy(gameObject);
    }

    Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player.gameObject)
        {
            Debug.Log("Hit");
            playerHealth.TakeDamage(10);
        }
        Destroy(gameObject);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
    }
}

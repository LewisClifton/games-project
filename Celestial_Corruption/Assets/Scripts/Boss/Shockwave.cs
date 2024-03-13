using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowTorus : MonoBehaviour
{
    [SerializeField] GameObject torusPrefab;
    [SerializeField] GameObject boss;
    [SerializeField] float maxScale;
    [SerializeField] float growSpeed;

    private GameObject torusInstance;

    // Start is called before the first frame update
    void Start()
    {
        SpawnTorus();
    }

    // Update is called once per frame
    void Update()
    {
        if (torusInstance != null)
        {
            // Calculate the current scale of the torus
            float currentScale = torusInstance.transform.localScale.x;

            // If the current scale of the torus is less than the maximum scale, continue growing
            if (currentScale < maxScale)
            {
                // Increase the scale based on growSpeed
                float newScale = currentScale + growSpeed * Time.deltaTime;

                // Limit the scale to not exceed the maximum value
                newScale = Mathf.Min(newScale, maxScale);

                // Update the size of the torus
                torusInstance.transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
    }

    // Spawn the Torus
    void SpawnTorus()
    {
        if (torusPrefab != null && boss != null)
        {
            // Get the position of the Boss
            Vector3 spawnPosition = new Vector3(boss.transform.position.x, 0f, boss.transform.position.z);

            // Spawn the Torus at the Boss's position and rotate it by 90 degrees
            torusInstance = Instantiate(torusPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
            torusInstance.SetActive(true);
        }
    }
}





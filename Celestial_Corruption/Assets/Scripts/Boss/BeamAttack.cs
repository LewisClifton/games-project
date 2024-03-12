using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttack : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject trapPrefab; // the circle show the trap
    [SerializeField] GameObject beamPrefab; // the beam gonna do damage
    [SerializeField] float maxX;
    [SerializeField] float maxZ;
    [SerializeField] float spawndistance; // distance between each of them
    [SerializeField] int number; // how many of these
    [SerializeField] float timeup; // the time for beam to show up
    [SerializeField] float timedown; // the time for beam to disappear
    [SerializeField] float beamheight; // the height of the beam
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
        if (Time.time - lastSpawnTime >= timeup)
        {
            Generatetrap();
            lastSpawnTime = Time.time; // Update last spawn time
        }
    }

    public void Generatetrap() 
    {
        for(int i = 0; i < number; i++) 
        {
            List<Vector3> trapPositions = new List<Vector3>();
            float bossX = boss.transform.position.x;
            float bossZ = boss.transform.position.z;
            Vector3 trapPosition = new Vector3(Random.Range(bossX - maxX, bossX + maxX), 0f, Random.Range(bossZ - maxZ, bossZ + maxZ));
            GameObject newTrap = Instantiate(trapPrefab, trapPosition, Quaternion.identity) as GameObject;

            newTrap.SetActive(true);
            Destroy(newTrap,timeup);

            // Save the trap position
            trapPositions.Add(trapPosition);
            Debug.Log(trapPositions);
        }
    }
}

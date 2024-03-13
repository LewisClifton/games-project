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
    [SerializeField] float trapDuration; // the duration of the trap
    [SerializeField] float beamDuration; // the duration of the beam
    [SerializeField] float beamHeight; // the height of the beam
    private float lastSpawnTime;
    private List<Vector3> trapPositions = new List<Vector3>(); // Keep track of trap positions
    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time; // Set last spawn time to current time to ensure immediate spawning
    }

    // Update is called once per frame
    void Update()
    {
        // Check if enough time has passed since the last spawn
        if (Time.time - lastSpawnTime >= (beamDuration + trapDuration))
        {
            Generatetrap();
            lastSpawnTime = Time.time; // Update last spawn time
        }
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
}






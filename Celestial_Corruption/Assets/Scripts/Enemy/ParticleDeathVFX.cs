using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple script to destroy gameobjects after a few seconds
public class ParticleDeathVFX : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, time);
    }
}

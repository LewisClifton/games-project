using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMInput
{
    [System.Serializable]
    public class MMInput
    {
        [SerializeField] public int tag;

        [Header("Directions")]
        [System.NonSerialized] public Vector3[] trajectory = new Vector3[3];
        [System.NonSerialized] public Vector3 acceleration;

        [Header("Lengths")]
        [SerializeField] public float defaultLength;

        [Header("Sensitivity")] 
        [SerializeField] public float acc;
        [SerializeField] public float decc;
    }
}

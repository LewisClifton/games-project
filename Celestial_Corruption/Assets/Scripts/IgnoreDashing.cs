using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreDashing : MonoBehaviour
{
    void Start()
    {
        Physics.IgnoreLayerCollision(0, 6);
    }
}

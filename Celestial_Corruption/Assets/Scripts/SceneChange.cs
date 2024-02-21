using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Initialsed");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided with : " + collision.gameObject.name);
        SceneManager.LoadScene("Dungeon");
    }

}

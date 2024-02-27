using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TimeFreeze : MonoBehaviour
{
    [SerializeField] float TimeScale = 0.1f;
    float normalTimeScale = 1f; // the normal time scale
    float fixedTime = 0f;
    float maxfixedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        fixedTime = Time.fixedDeltaTime;
        maxfixedTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // Press and hold the "I" key to make time flow slower
            Time.timeScale = TimeScale;
        }
        else
        {
            // Release the "I" key to resume normal time flow rate
            Time.timeScale = normalTimeScale;
        }
    }
    public void ChangeTimeScale()
    {
        Time.timeScale = TimeScale;   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TimeFreeze : MonoBehaviour
{
    [SerializeField] float TimeScale = 0.05f;
    float normalTimeScale = 1f; // the normal time scale
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // Right click to make time flow slower
            Time.timeScale = TimeScale;
        }
        else
        {
            // Release the mouse to resume normal time flow rate
            Time.timeScale = normalTimeScale;
        }
    }
    public void ChangeTimeScale()
    {
        Time.timeScale = TimeScale;   
    }
}

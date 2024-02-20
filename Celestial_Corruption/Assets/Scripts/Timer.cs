using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timerTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerTime += Time.deltaTime;
        UpdateText();
    }

    private void UpdateText()
    {
        int minutes = (int)timerTime / 60;
        int seconds = (int)timerTime % 60;
        float milliSeconds = 100 * (timerTime % 1);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds,milliSeconds);
        
    }
}

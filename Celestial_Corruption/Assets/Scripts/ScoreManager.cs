using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    private float score;
    public float scoreMultiplier = 1;
    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMultiplier(float amount)
    {
        scoreMultiplier += amount;
    }

    public void ResetMultiplier()
    {
        scoreMultiplier = 1;
    }

    public void AddScore(int amount)
    {
        score += amount*scoreMultiplier;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score:" + score;
    }
}

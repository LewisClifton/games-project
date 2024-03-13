using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    private float score;
    public int scoreMultiplier = 1;
    public float multiplierIncreaseRequirement = 1000;
    public float multiplierIncreaseRequirementProgress = 0;
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

    public void AddMultiplier(int amount)
    {
        scoreMultiplier += amount;
    }

    public void ResetMultiplier()
    {
        scoreMultiplier = 1;
        multiplierIncreaseRequirementProgress = 0;
    }

    public void AddScore(int amount)
    {
        
        score += amount*scoreMultiplier;
        multiplierIncreaseRequirementProgress += amount;
        if (multiplierIncreaseRequirementProgress > multiplierIncreaseRequirement * scoreMultiplier)
        {
            AddMultiplier(1);
            multiplierIncreaseRequirementProgress -= multiplierIncreaseRequirement * (scoreMultiplier - 1);
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score:" + score;
    }

    public int GetMultiplier()
    {
        return scoreMultiplier;
    }
}

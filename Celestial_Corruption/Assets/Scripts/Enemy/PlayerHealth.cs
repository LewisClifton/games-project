using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100;

    private void Update()
    {
        if (healthAmount <= 0)
        {
            Application.LoadLevel(Application.loadedLevel); // Reload the secene I guess
        }

        
        /*      Just the Keypress to test if the health bar works

                if (Input.GetKeyDown(KeyCode.J))
                {
                    TakeDamage(20);
                }

                if (Input.GetKeyDown(KeyCode.K))
                {
                    Healing(10);
                }
        */

    }

    public void TakeDamage(float Damage)
    {
        healthAmount -= Damage;
        healthBar.fillAmount = healthAmount / 100;
    }

    public void Healing(float healPoints)
    {
        healthAmount += healPoints;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100;
    }

}
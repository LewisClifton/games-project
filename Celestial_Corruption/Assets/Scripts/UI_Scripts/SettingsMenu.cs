using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameInput gameInput;
    [SerializeField] private InputActionAsset inputActionAsset;

    [SerializeField] private GameObject CamSensitivityMenu;
    [SerializeField] private GameObject KeybindsMenu;

    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject LeftArrow;
    [SerializeField] private GameObject RightArrow;
    [SerializeField] private GameObject[] settingsWindows;
    [SerializeField] private GameObject[] settingsButtons;
    private int currentSettingsWindow = 0;

    void Awake()    
    {
        // CamSensitivityMenu.SetActive(true);
        // KeybindsMenu.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        // Activates buttons
        LeftArrow.SetActive(true);
        RightArrow.SetActive(true);
        for (int i = 0; i < settingsButtons.Length; i++)
        {
            settingsButtons[i].SetActive(true);
        }

        SetSettingsWindow(0);
    }

    public void SetSettingsWindow(int window)
    {
        Debug.Log("Setting window to " + window);
        settingsWindows[currentSettingsWindow].SetActive(false);

        settingsWindows[window].SetActive(true);
        currentSettingsWindow = window;
    }

    public void rightArrow()
    {
        settingsWindows[currentSettingsWindow].SetActive(false);
        currentSettingsWindow = (currentSettingsWindow + 1) % settingsWindows.Length;
        settingsWindows[currentSettingsWindow].SetActive(true);
    }

    public void leftArrow()
    {
        settingsWindows[currentSettingsWindow].SetActive(false);
        currentSettingsWindow = (currentSettingsWindow - 1 + settingsWindows.Length) % settingsWindows.Length;
        settingsWindows[currentSettingsWindow].SetActive(true);
    }

    public void ChangeToCamSensitivityMenu()
    {
        settingsWindows[currentSettingsWindow].SetActive(false);
        currentSettingsWindow = 0;
        settingsWindows[currentSettingsWindow].SetActive(true);
    }

    public void ChangeToKeybindsMenu()
    {
        settingsWindows[currentSettingsWindow].SetActive(false);
        currentSettingsWindow = 1;
        settingsWindows[currentSettingsWindow].SetActive(true);
    }
}

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

        SetSettingsWindow(0);
    }

    public void SetSettingsWindow(int window)
    {
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
}

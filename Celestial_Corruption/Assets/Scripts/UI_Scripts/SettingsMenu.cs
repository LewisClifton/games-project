using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Cinemachine.CinemachineFreeLook playerCamera;
    [SerializeField] private Slider sensitivitySliderX;
    [SerializeField] private Slider sensitivitySliderY;

    private Slider sliderX;
    private Slider sliderY;
    private TMP_InputField sliderValueX;
    private TMP_InputField sliderValueY;

    private float cameraSensitivityXaxis;
    private float cameraSensitivityYaxis;
    private float cameraSensitivityConstantX;
    private float cameraSensitivityConstantY;

    void Awake()
    {
        sliderX = sensitivitySliderX;
        sliderY = sensitivitySliderY;
        sliderValueX = sensitivitySliderX.GetComponentInChildren<TMP_InputField>();
        sliderValueY = sensitivitySliderY.GetComponentInChildren<TMP_InputField>();
    }

    void Start()
    {
        cameraSensitivityXaxis = Settings.cameraSensitivityXaxis;
        cameraSensitivityYaxis = Settings.cameraSensitivityYaxis;

        cameraSensitivityConstantX = Settings.cameraSensitivityConstantX;
        cameraSensitivityConstantY = Settings.cameraSensitivityConstantY;

        if (playerCamera != null)
        {
            playerCamera.m_XAxis.m_MaxSpeed = cameraSensitivityXaxis;
            playerCamera.m_YAxis.m_MaxSpeed = cameraSensitivityYaxis;
        }

        // cameraSensitivityXaxis = playerCamera.m_XAxis.m_MaxSpeed;
        // cameraSensitivityYaxis = playerCamera.m_YAxis.m_MaxSpeed;

        // cameraSensitivityConstantX = cameraSensitivityXaxis;
        // cameraSensitivityConstantY = cameraSensitivityYaxis;

        sliderValueX.text = cameraSensitivityXaxis.ToString();
        sliderValueY.text = cameraSensitivityYaxis.ToString();

        sliderX.value = cameraSensitivityXaxis / cameraSensitivityConstantX;
        sliderY.value = cameraSensitivityYaxis / cameraSensitivityConstantY;
    }

    public void SetCameraSensitivityX(float sensitivity)
    {
        Debug.Log("Camera sensitivity X set to: " + sensitivity);
        if (playerCamera != null)
        {
            playerCamera.m_XAxis.m_MaxSpeed = sensitivity * cameraSensitivityConstantX;
        }
        sliderValueX.text = (sensitivity * cameraSensitivityConstantX).ToString();
        cameraSensitivityXaxis = sensitivity * cameraSensitivityConstantX;
        Settings.cameraSensitivityXaxis = cameraSensitivityXaxis;
    }

    public void SetCameraSensitivityY(float sensitivity)
    {
        Debug.Log("Camera sensitivity Y set to: " + sensitivity);
        if (playerCamera != null)
        {
            playerCamera.m_YAxis.m_MaxSpeed = sensitivity * cameraSensitivityConstantY;
        }
        sliderValueY.text = (sensitivity * cameraSensitivityConstantY).ToString();
        cameraSensitivityYaxis = sensitivity * cameraSensitivityConstantY;
        Settings.cameraSensitivityYaxis = cameraSensitivityYaxis;
    }

    public void SetCameraSensitivityX(string sensitivity)
    {
        float value;
        if (float.TryParse(sensitivity, out value))
        {
            Debug.Log("Camera sensitivity X set to: " + value);
            if (playerCamera != null)
            {
                playerCamera.m_XAxis.m_MaxSpeed = value;
            }
            sliderValueX.text = value.ToString();
            sliderX.value = value / cameraSensitivityConstantX;
            cameraSensitivityXaxis = value;
            Settings.cameraSensitivityXaxis = cameraSensitivityXaxis;
        }
    }

    public void SetCameraSensitivityY(string sensitivity)
    {
        float value;
        if (float.TryParse(sensitivity, out value))
        {
            Debug.Log("Camera sensitivity Y set to: " + value);
            // If there is no player Camera assigned, then don't do anything
            if (playerCamera != null)
            {
                playerCamera.m_YAxis.m_MaxSpeed = value;
            }
            sliderValueY.text = value.ToString();
            sliderY.value = value / cameraSensitivityConstantY;
            cameraSensitivityYaxis = value;
            Settings.cameraSensitivityYaxis = cameraSensitivityYaxis;
        }
    }

    // public void SetSensitivityX(float sensitivity)
    // {
    //     playerCamera.m_XAxis.m_MaxSpeed = sensitivity;
    // }

    // Not Used
    public void Save()
    {
        cameraSensitivityXaxis = playerCamera.m_XAxis.m_MaxSpeed;
        cameraSensitivityYaxis = playerCamera.m_YAxis.m_MaxSpeed; 
    }

    // Not Used
    public void Cancel()
    {
        playerCamera.m_XAxis.m_MaxSpeed = cameraSensitivityXaxis;
        playerCamera.m_YAxis.m_MaxSpeed = cameraSensitivityYaxis;
    }

    public void InitialSettings()
    {

    }

    public void Default()
    {
        if (playerCamera != null)
        {
            playerCamera.m_XAxis.m_MaxSpeed = cameraSensitivityConstantX;
            playerCamera.m_YAxis.m_MaxSpeed = cameraSensitivityConstantY;
        }
        cameraSensitivityXaxis = playerCamera.m_XAxis.m_MaxSpeed;
        cameraSensitivityYaxis = playerCamera.m_YAxis.m_MaxSpeed;
        sliderValueX.text = playerCamera.m_XAxis.m_MaxSpeed.ToString();
        sliderValueY.text = playerCamera.m_YAxis.m_MaxSpeed.ToString();
        sliderX.value = 1;
        sliderY.value = 1;
        Settings.cameraSensitivityXaxis = cameraSensitivityXaxis;
        Settings.cameraSensitivityYaxis = cameraSensitivityYaxis;
    }
}

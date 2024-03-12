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
        cameraSensitivityXaxis = playerCamera.m_XAxis.m_MaxSpeed;
        cameraSensitivityYaxis = playerCamera.m_YAxis.m_MaxSpeed;

        cameraSensitivityConstantX = cameraSensitivityXaxis;
        cameraSensitivityConstantY = cameraSensitivityYaxis;

        sliderValueX.text = cameraSensitivityXaxis.ToString();
        sliderValueY.text = cameraSensitivityYaxis.ToString();

        Debug.Log("Camera sensitivity X: " + cameraSensitivityXaxis);
        Debug.Log("Camera sensitivity Y: " + cameraSensitivityYaxis);
    }

    public void SetCameraSensitivityX(float sensitivity)
    {
        Debug.Log("Camera sensitivity X set to: " + sensitivity);
        playerCamera.m_XAxis.m_MaxSpeed = sensitivity * cameraSensitivityConstantX;
        sliderValueX.text = playerCamera.m_XAxis.m_MaxSpeed.ToString();
    }

    public void SetCameraSensitivityY(float sensitivity)
    {
        Debug.Log("Camera sensitivity Y set to: " + sensitivity);
        playerCamera.m_YAxis.m_MaxSpeed = sensitivity * cameraSensitivityConstantY;
        sliderValueY.text = playerCamera.m_YAxis.m_MaxSpeed.ToString();
    }

    public void SetCameraSensitivityX(string sensitivity)
    {
        float value;
        if (float.TryParse(sensitivity, out value))
        {
            playerCamera.m_XAxis.m_MaxSpeed = value;
            sliderValueX.text = playerCamera.m_XAxis.m_MaxSpeed.ToString();
            sliderX.value = value / cameraSensitivityConstantX;
        }
    }

    public void SetCameraSensitivityY(string sensitivity)
    {
        float value;
        if (float.TryParse(sensitivity, out value))
        {
            playerCamera.m_YAxis.m_MaxSpeed = value;
            sliderValueY.text = playerCamera.m_YAxis.m_MaxSpeed.ToString();
            sliderY.value = value / cameraSensitivityConstantY;
        }
    }

    // public void SetSensitivityX(float sensitivity)
    // {
    //     playerCamera.m_XAxis.m_MaxSpeed = sensitivity;
    // }

    public void Save()
    {
        cameraSensitivityXaxis = playerCamera.m_XAxis.m_MaxSpeed;
        cameraSensitivityYaxis = playerCamera.m_YAxis.m_MaxSpeed; 
    }

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
        playerCamera.m_XAxis.m_MaxSpeed = 200f;
        playerCamera.m_YAxis.m_MaxSpeed = 1.3f;
        sliderValueX.text = playerCamera.m_XAxis.m_MaxSpeed.ToString();
        sliderValueY.text = playerCamera.m_YAxis.m_MaxSpeed.ToString();
        sliderX.value = 1;
        sliderY.value = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Cinemachine.CinemachineFreeLook playerCamera;
    [SerializeField] private GameObject sensitivitySliderX;
    [SerializeField] private GameObject sensitivitySliderY;

    private GameObject sliderX;
    private GameObject sliderY;
    private TMP_InputField sliderValueX;
    private TMP_InputField sliderValueY;

    private float cameraSensitivityXaxis;
    private float cameraSensitivityYaxis;

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
        sliderValueX.text = cameraSensitivityXaxis.ToString();
        sliderValueY.text = cameraSensitivityYaxis.ToString();

        Debug.Log("Camera sensitivity X: " + cameraSensitivityXaxis);
        Debug.Log("Camera sensitivity Y: " + cameraSensitivityYaxis);
    }

    public void SetCameraSensitivityX(float sensitivity)
    {
        Debug.Log("Camera sensitivity X set to: " + sensitivity);
        playerCamera.m_XAxis.m_MaxSpeed = sensitivity;
        sliderValueX.text = sensitivity.ToString();
    }

    public void SetCameraSensitivityY(float sensitivity)
    {
        Debug.Log("Camera sensitivity Y set to: " + sensitivity);
        playerCamera.m_YAxis.m_MaxSpeed = sensitivity;
        sliderValueY.text = sensitivity.ToString();
    }

    // public void SetSensitivityX(float sensitivity)
    // {
    //     playerCamera.m_XAxis.m_MaxSpeed = sensitivity;
    // }

    public void ShowSensitivityX()
    {

    }

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
}

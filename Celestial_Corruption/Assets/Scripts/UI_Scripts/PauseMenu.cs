using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    private bool isPaused = false;

    void Start()
    {
        PauseMenuDeactivate();
        settingsMenu.SetActive(false);
    }

    void Update()
    {
        if (gameInput.IsEscapePressed())
        {
            if (isPaused)
            {
                if (settingsMenu.activeSelf)
                {
                    settingsMenu.SetActive(false);
                    PauseMenuActivate();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseMenuDeactivate()
    {
        pauseMenu.GetComponent<Image>().enabled = false;
        // Loop over all the children of the pauseMenu and disable them
        foreach (Transform child in pauseMenu.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void PauseMenuActivate()
    {
        pauseMenu.GetComponent<Image>().enabled = true;
        // Loop over all the children of the pauseMenu and enable them
        foreach (Transform child in pauseMenu.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void PauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
        PauseMenuActivate();
        isPaused = true;
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;
        PauseMenuDeactivate();
        isPaused = false;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        // Don't trigger this in the editor
        Application.Quit();
    }

    public void RestartGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OpenSettings()
    {
        PauseMenuDeactivate();
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        PauseMenuActivate();
    }
}

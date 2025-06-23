using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseGameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private PlayerAvatarController playerAvatarController;
    private GameObject clickedButton;
    public void gameButtonActions()
    {
        clickedButton = EventSystem.current.currentSelectedGameObject;
        Time.timeScale = 1;

        if (clickedButton.name == "PauseBttn")
        { 
            pauseScreen.SetActive(true);
            playerAvatarController.enabled = false;
            Time.timeScale = 0;
        }
        else if (clickedButton.name == "RestartBttn")
        {
            resumeGameplay();
            SceneManager.LoadScene("RicochetRival_Main");
        }
        else if (clickedButton.name == "MainMenuBttn")
        {
            resumeGameplay();
            SceneManager.LoadScene("RicochetRival_Home");
        }
        else if (clickedButton.name == "ResumeBttn")
        {
            resumeGameplay();
        }
        else
        {
            Application.Quit();
        }

    }

    private void resumeGameplay()
    {
        playerAvatarController.enabled = true;
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;
    public GameObject homeCanvas;
    public GameObject difficultyCanvas;

    public void PlayGame()
    {
        Debug.Log("Play button clicked.");

        if (homeCanvas != null) homeCanvas.SetActive(false);
        if (difficultyCanvas != null) difficultyCanvas.SetActive(true);
    }

    public void OpenHowToPlay()
    {
        Debug.Log("How to Play button clicked.");
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button clicked.");
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked.");
        Application.Quit();
    }

    public void TestClick()
    {
        Debug.Log("Button clicked!");
    }
}

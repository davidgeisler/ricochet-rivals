using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class buttonsActions : MonoBehaviour
{
    [SerializeField] private GameObject howToHome;
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject bounceRules;
    [SerializeField] private GameObject shotLogic;
    [SerializeField] private GameObject hyperMode;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject backBttn;
    private GameObject clickedButton;

    public void buttons()
    {
        clickedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log(clickedButton.name);

        if (clickedButton.name == "HowToPlay")
        {
            mainMenu.SetActive(false);
            backBttn.SetActive(true);
            howToHome.SetActive(true);
        }
        else if (clickedButton.name == "Settings")
        {
            mainMenu.SetActive(false);
            settings.SetActive(true);
        }
        else if (clickedButton.name == "btnBounceRules")
        {
            bounceRules.SetActive(true);
            howToHome.SetActive(false);
        }
        else if (clickedButton.name == "btnShotLogic")
        {
            shotLogic.SetActive(true);
            howToHome.SetActive(false);
        }
        else if (clickedButton.name == "btnControls")
        {
            controls.SetActive(true);
            howToHome.SetActive(false);
        }
        else if (clickedButton.name == "btnHyperMode")
        {
            hyperMode.SetActive(true);
            howToHome.SetActive(false);
        }
        else
        {
            if (howToHome.activeSelf)
            {
                mainMenu.SetActive(true);
                backBttn.SetActive(false);
                howToHome.SetActive(false);
            }
            else if(settings.activeSelf)
            {
                mainMenu.SetActive(true);
                backBttn.SetActive(false);
                settings.SetActive(false);
            }
            else
            {
                howToHome.SetActive(true);
                controls.SetActive(false);
                bounceRules.SetActive(false);
                shotLogic.SetActive(false);
                hyperMode.SetActive(false);
            }
        }
    }
}

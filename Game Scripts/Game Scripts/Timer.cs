using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float minutes;
    private float currentTime;
    private bool timerActive;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private HyperModeManager hyperModeManager;
    [SerializeField] private GameObject gameOverScreen;
    private GameObject playerControl;

    private bool hyperTriggered = false;

    void Start()
    {
        playerControl = GameObject.FindWithTag("Player");
        currentTime = minutes * 60;
        startTimer();

        if (hyperModeManager != null && hyperModeManager.blurOverlay != null)
        {
            hyperModeManager.blurOverlay.SetActive(false);
        }
    }

    void Update()
    {
        if (timerActive)
        {
            if (currentTime <= 0)
            {
                stopTimer();
                gameOverScreen.SetActive(true);
                Time.timeScale = 0;
                playerControl.GetComponent<PlayerAvatarController>().enabled = false;
            }
            else
            {
                currentTime -= Time.deltaTime;

                if (currentTime <= 30f && !hyperTriggered)
                {
                    if (hyperModeManager != null)
                    {
                        Debug.Log(">>> Hyper Mode Triggered!");
                        hyperModeManager.ActivateHyperMode();
                        hyperTriggered = true;
                    }
                }
            }

            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            timerText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString("00");
        }
    }

    public void stopTimer() => timerActive = false;
    public void startTimer() => timerActive = true;
}

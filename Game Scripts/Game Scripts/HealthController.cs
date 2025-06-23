using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public enum Owner { Player, AI }
    public Owner owner;

    public GameObject[] hearts; // Assigned in reverse order (3, 2, 1)

    private int currentHealth;
    private Animator animator;

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private PlayerAvatarController playerAvatarController;

    void Start()
    {
        currentHealth = hearts.Length;
        animator = GetComponent<Animator>(); // Optional: for shake animation
    }

    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;

        // Deactivate current heart
        if (hearts[currentHealth] != null)
            hearts[currentHealth].SetActive(false);

        // Trigger optional shake animation
        if (animator != null)
            animator.SetTrigger("Shake");

        // If health reaches 0
        if (currentHealth == 0)
        {
            if(owner == 0)
            {
                gameOverScreen.SetActive(true);
                Time.timeScale = 0;
                playerAvatarController.enabled = false;
            } else
            {
                victoryScreen.SetActive(true);
                Time.timeScale = 0;
                playerAvatarController.enabled = false;
            }
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}

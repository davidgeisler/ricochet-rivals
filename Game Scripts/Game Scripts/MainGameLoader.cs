using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameLoader : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainGameCanvas;
    public GameObject difficultySelectionCanvas;
    public GameObject homeCanvas;

    [Header("Audio")]
    public AudioSource inGameMusic;  // 🎯 Use this AudioSource directly
    public float fadeInDuration = 2f;

    void Start()
    {
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "");

        if (!string.IsNullOrEmpty(difficulty))
        {
            // Enable MainGame UI
            if (mainGameCanvas != null)
                mainGameCanvas.SetActive(true);

            // Hide old menus
            if (difficultySelectionCanvas != null)
                difficultySelectionCanvas.SetActive(false);

            if (homeCanvas != null)
                homeCanvas.SetActive(false);

            // Assign AI difficulty
            AssignDifficultyToAI(difficulty);

            // Start In-Game Music
            PlayInGameMusic();
        }
        else
        {
            if (mainGameCanvas != null)
                mainGameCanvas.SetActive(false);
        }
    }

    void AssignDifficultyToAI(string difficulty)
    {
        GameObject ai = GameObject.Find("AIAvatar");
        if (ai == null) return;

        var aiController = ai.GetComponent<EnemyAIController>();
        if (aiController == null) return;

        switch (difficulty)
        {
            case "Easy":
                aiController.difficulty = EnemyAIController.AILevel.Easy;
                break;
            case "Medium":
                aiController.difficulty = EnemyAIController.AILevel.Medium;
                break;
            case "Hard":
                aiController.difficulty = EnemyAIController.AILevel.Hard;
                break;
            default:
                aiController.difficulty = EnemyAIController.AILevel.Easy;
                break;
        }

        aiController.RefreshDifficultyText();
    }

    void PlayInGameMusic()
    {
        // Stop previous persistent music if any
        var oldMusic = GameObject.Find("HomeMusicManager");
        if (oldMusic != null) Destroy(oldMusic);

        if (inGameMusic != null)
        {
            inGameMusic.volume = 0f;
            inGameMusic.loop = true;
            inGameMusic.Play();

            DontDestroyOnLoad(inGameMusic.gameObject);
            StartCoroutine(FadeInMusic());
        }
    }

    IEnumerator FadeInMusic()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            inGameMusic.volume = Mathf.Lerp(0f, 0.6f, t / fadeInDuration);
            yield return null;
        }

        inGameMusic.volume = 0.6f;
    }
}

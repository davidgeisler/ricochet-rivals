using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HyperModeManager : MonoBehaviour
{
    [Header("Hyper Mode Settings")]
    public float duration = 30f;
    public float fireRate = 0.8f;
    public int maxBounces = 8;

    [Header("References")]
    public PlayerAvatarController playerController;
    public EnemyAIController aiController;
    public SpriteRenderer backgroundRenderer;
    public Sprite hyperBG;
    public Sprite normalBG;
    public GameObject hyperLogo;
    public GameObject blurOverlay;

    [Header("Music")]
    public AudioSource inGameMusic;
    public AudioSource hyperMusic;
    public float fadeDuration = 1.5f;

    [Header("Corner Logo")]
    public GameObject cornerLogo;
    private CanvasGroup cornerGroup;

    private float originalFireRate;
    private Vector3 originalCamPos;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        originalCamPos = cam.transform.position;

        if (cornerLogo != null)
        {
            cornerGroup = cornerLogo.GetComponent<CanvasGroup>();
            if (cornerGroup != null)
            {
                cornerGroup.alpha = 0f;
                cornerLogo.SetActive(false);
            }
        }
    }

    public void ActivateHyperMode()
    {
        StartCoroutine(HyperRoutine());
    }

    IEnumerator HyperRoutine()
    {
        // Fade out In-Game OST and fade in Hyper Mode music starting at 1:04
        if (inGameMusic != null && hyperMusic != null)
        {
            StartCoroutine(FadeOutAudio(inGameMusic, fadeDuration));

            hyperMusic.time = 64f; // start at 1:04
            yield return StartCoroutine(FadeInAudio(hyperMusic, fadeDuration));
        }

        blurOverlay.SetActive(true);
        hyperLogo.SetActive(true);

        CanvasGroup logoGroup = hyperLogo.GetComponent<CanvasGroup>();
        CanvasGroup blurGroup = blurOverlay.GetComponent<CanvasGroup>();

        logoGroup.alpha = 1;
        blurGroup.alpha = 1;

        yield return new WaitForSeconds(1.5f);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            logoGroup.alpha = alpha;
            blurGroup.alpha = alpha;
            yield return null;
        }

        hyperLogo.SetActive(false);
        blurOverlay.SetActive(false);

        if (cornerLogo != null && cornerGroup != null)
        {
            cornerLogo.SetActive(true);
            StartCoroutine(FadeInCornerLogo());
        }

        backgroundRenderer.sprite = hyperBG;
        originalFireRate = playerController.shootInterval;
        playerController.shootInterval = fireRate;
        if (aiController != null) aiController.shootInterval = fireRate;
        PlayerProjectile.globalMaxBounces = maxBounces;

        StartCoroutine(CameraShake(duration));
        yield return new WaitForSeconds(duration);

        // Fade out Hyper Music and fade in In-Game Music again
        if (hyperMusic != null && inGameMusic != null)
        {
            StartCoroutine(FadeOutAudio(hyperMusic, fadeDuration));
            yield return StartCoroutine(FadeInAudio(inGameMusic, fadeDuration));
        }

        playerController.shootInterval = originalFireRate;
        if (aiController != null) aiController.shootInterval = originalFireRate;
        PlayerProjectile.globalMaxBounces = 3;
        backgroundRenderer.sprite = normalBG;
        cam.transform.position = originalCamPos;
    }

    IEnumerator CameraShake(float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            cam.transform.position = originalCamPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = originalCamPos;
    }

    IEnumerator FadeInCornerLogo()
    {
        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            cornerGroup.alpha = alpha;
            yield return null;
        }

        cornerGroup.alpha = 1f;
    }

    IEnumerator FadeOutAudio(AudioSource audio, float duration)
    {
        float startVolume = audio.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        audio.volume = 0f;
        audio.Pause();
    }

    IEnumerator FadeInAudio(AudioSource audio, float duration)
    {
        audio.Play();
        float t = 0f;
        audio.volume = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            audio.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        audio.volume = 1f;
    }
}

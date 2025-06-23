using UnityEngine;
using System.Collections;

public class HomeMusicManager : MonoBehaviour
{
    public AudioSource audioSource;

    [SerializeField] private AudioClip homeOST;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = homeOST;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();

        //StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float duration = 1.5f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 0.5f, t / duration);
            yield return null;
        }

        audioSource.volume = 0.5f;
    }
}

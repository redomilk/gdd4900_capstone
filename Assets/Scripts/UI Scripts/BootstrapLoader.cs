using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "Main Menu";
    [SerializeField] private float minimumLoadTime = 1f;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private IEnumerator Start()
    {
        // Start transparent
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        // Wait while bootstrap/loading is visible
        yield return new WaitForSeconds(minimumLoadTime);

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(firstSceneName);
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Clamp01(t / fadeDuration);

            fadeImage.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        fadeImage.color = Color.black;
    }
}
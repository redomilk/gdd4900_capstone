using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    public Image fadeImage;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public IEnumerator FadeToBlack(float duration)
    {
        fadeImage.gameObject.SetActive(true);

        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Clamp01(t / duration);

            fadeImage.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        fadeImage.color = Color.black;
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        fadeImage.gameObject.SetActive(true);

        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = 1f - Mathf.Clamp01(t / duration);

            fadeImage.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject menuCanvas;

    [Header("Dive Transition")]
    public GameObject bubblePrefab;
    public float scrollDuration = 3f;
    public float diveDistance = 18f;
    public float bubbleLingerTime = 2f;
    public float spawnWidth = 10f;

    private bool gameStarted = false;

    void Start()
    {
        menuCanvas.SetActive(true);
    }

    public void OnPlayPressed()
    {
        if (!gameStarted)
            StartCoroutine(DiveTransition());
    }

    IEnumerator DiveTransition()
    {
        gameStarted = true;
        yield return StartCoroutine(FadeOutMenu());
        StartCoroutine(ScrollCamera());
        StartCoroutine(SpawnBubbles(scrollDuration + bubbleLingerTime));
        yield return new WaitForSeconds(scrollDuration + bubbleLingerTime);
        SceneManager.LoadScene("SQ scene");
    }

    IEnumerator FadeOutMenu()
    {
        CanvasGroup cg = menuCanvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = menuCanvas.AddComponent<CanvasGroup>();
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            cg.alpha = 1f - t;
            yield return null;
        }
        menuCanvas.SetActive(false);
    }

    IEnumerator ScrollCamera()
    {
        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = startPos + Vector3.down * diveDistance;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / scrollDuration;
            float eased = 1f - Mathf.Pow(1f - t, 2f);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }
    }

    IEnumerator SpawnBubbles(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float progress = Mathf.Clamp01(elapsed / duration);

            if (progress > 0.15f)
            {
                float spawnProgress = (progress - 0.15f) / 0.85f;
                float spawnRate = Mathf.Lerp(0.5f, 0.08f, spawnProgress);

                // Spawn 1 bubble early on, up to 5 at full depth
                int count = Mathf.RoundToInt(Mathf.Lerp(1f, 5f, spawnProgress));
                for (int i = 0; i < count; i++)
                    SpawnOneBubble(progress);

                yield return new WaitForSeconds(spawnRate);
                elapsed += spawnRate;
            }
            else
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    void SpawnOneBubble(float progress)
    {
        Vector3 camPos = Camera.main.transform.position;
        float x = camPos.x + Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float y = camPos.y - 5f;
        GameObject b = Instantiate(bubblePrefab, new Vector3(x, y, 0f), Quaternion.identity);
        StartCoroutine(FloatBubble(b, progress));
    }

    IEnumerator FloatBubble(GameObject bubble, float depthProgress)
    {
        float speed = Random.Range(1.5f, 4f);
        float wobble = Random.Range(0.3f, 1f);
        float lifetime = Random.Range(2f, 5f);
        float t = 0f;
        Vector3 startPos = bubble.transform.position;
        SpriteRenderer sr = bubble.GetComponent<SpriteRenderer>();

        // White near surface, dark blue-grey at depth
        Color shallowColor = new Color(1f, 1f, 1f);
        Color deepColor = new Color(0.3f, 0.4f, 0.5f);
        Color bubbleColor = Color.Lerp(shallowColor, deepColor, depthProgress);

        while (t < lifetime)
        {
            t += Time.deltaTime;
            float x = startPos.x + Mathf.Sin(t * wobble * 3f) * 0.3f;
            float y = startPos.y + t * speed;
            bubble.transform.position = new Vector3(x, y, 0f);

            if (sr != null)
                sr.color = new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, 1f - (t / lifetime));

            yield return null;
        }
        Destroy(bubble);
    }
}
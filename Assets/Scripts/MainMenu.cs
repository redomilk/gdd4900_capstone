using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject player;

    [Header("HUD Elements")]
    public GameObject depthGauge;
    public GameObject resourceBars;

    [Header("Dive Transition")]
    public GameObject bubblePrefab;
    public float scrollDuration = 3f;      // how long the camera scrolls
    public float diveDistance = 18f;       // how far down to scroll (into your dark BG)
    public float bubbleLingerTime = 2f;    // extra bubble time after camera stops
    public float spawnWidth = 10f;

    private bool gameStarted = false;

    void Start()
    {
        menuCanvas.SetActive(true);
        if (player != null) player.SetActive(false);
        if (depthGauge != null) depthGauge.SetActive(false);
        if (resourceBars != null) resourceBars.SetActive(false);
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

        // Load your game scene instead of enabling objects
        UnityEngine.SceneManagement.SceneManager.LoadScene("Ocean Test");
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
            // Ease-out: starts fast, slows to a stop in the dark
            float eased = 1f - Mathf.Pow(1f - t, 2f);
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }
    }

    IEnumerator SpawnBubbles(float duration)
    {
        float elapsed = 0f;
        float spawnRate = 0.12f;

        while (elapsed < duration)
        {
            SpawnOneBubble();
            yield return new WaitForSeconds(spawnRate);
            elapsed += spawnRate;
        }
    }

    void SpawnOneBubble()
    {
        Vector3 camPos = Camera.main.transform.position;
        float x = camPos.x + Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float y = camPos.y - 5f; // spawn below camera, float up through frame
        GameObject b = Instantiate(bubblePrefab, new Vector3(x, y, 0f), Quaternion.identity);
        StartCoroutine(FloatBubble(b));
    }

    IEnumerator FloatBubble(GameObject bubble)
    {
        float speed = Random.Range(1.5f, 4f);
        float wobble = Random.Range(0.3f, 1f);
        float lifetime = Random.Range(2f, 5f);
        float t = 0f;
        Vector3 startPos = bubble.transform.position;
        SpriteRenderer sr = bubble.GetComponent<SpriteRenderer>();

        while (t < lifetime)
        {
            t += Time.deltaTime;
            float x = startPos.x + Mathf.Sin(t * wobble * 3f) * 0.3f;
            float y = startPos.y + t * speed;
            bubble.transform.position = new Vector3(x, y, 0f);

            if (sr != null)
                sr.color = new Color(1f, 1f, 1f, 1f - (t / lifetime));

            yield return null;
        }
        Destroy(bubble);
    }

    public void ReturnToMenu()
    {
        gameStarted = false;
        menuCanvas.SetActive(true);
        player.SetActive(false);
        depthGauge.SetActive(false);
        resourceBars.SetActive(false);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;

public class MainMenu : MonoBehaviour
{
    public GameObject menuCanvas;

    [Header("Dive Transition")]
    public GameObject bubblePrefab;
    public float scrollDuration = 3f;
    public float diveDistance = 18f;
    public float bubbleLingerTime = 2f;
    public float spawnWidth = 10f;

    [Header("UI")]
    public GameObject optionsPanel;
    public GameObject mainMenuPanel;

    [Header("Screen Fade")]
    public Image fadeImage;
    public float fadeToBlackDuration = 1f;

    [Header("Fade")]
    public float panelFadeDuration = 0.15f;

    [Header("Save Slots")]
    public GameObject saveSlotPanel;

    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;
    public Button savePanelPlayButton;

    public Color normalSlotColor = Color.white;
    public Color selectedSlotColor = Color.yellow;

    public Color playDisabledColor = Color.gray;
    public Color playEnabledColor = Color.white;

    private int selectedSaveSlot = -1;

    private bool gameStarted = false;

    Coroutine panelFadeRoutine;

    void Start()
    {
        menuCanvas.SetActive(true);

        if (saveSlotPanel != null)
            saveSlotPanel.SetActive(false);

        selectedSaveSlot = -1;

        if (savePanelPlayButton != null)
        {
            savePanelPlayButton.interactable = false;
            SetButtonColor(savePanelPlayButton, playDisabledColor);
        }

        RefreshSlotHighlights();
    }

    public void OnPlayPressed()
    {
        if (gameStarted) return;

        if (saveSlotPanel != null)
            saveSlotPanel.SetActive(true);
    }

    IEnumerator DiveTransition()
    {
        gameStarted = true;

        //play ubbles sfx
        RuntimeManager.PlayOneShot("event:/SFX_menuBubbles");

        yield return StartCoroutine(FadeOutMenu());

        StartCoroutine(ScrollCamera());
        StartCoroutine(SpawnBubbles(scrollDuration + bubbleLingerTime));

        // wait until near the end of the dive
        yield return new WaitForSeconds(scrollDuration + bubbleLingerTime - fadeToBlackDuration);

        // fade screen to black
        yield return StartCoroutine(FadeToBlack());

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

    public void OpenOptions()
    {
        if (mainMenuPanel == null || optionsPanel == null) return;

        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (mainMenuPanel == null || optionsPanel == null) return;

        optionsPanel.SetActive(false);
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

    //save menu ui functions
    public void SelectSaveSlot(int slot)
    {
        if (gameStarted) return;

        selectedSaveSlot = slot;

        RefreshSlotHighlights();

        if (savePanelPlayButton != null)
        {
            savePanelPlayButton.interactable = true;
            SetButtonColor(savePanelPlayButton, playEnabledColor);
        }
    }

    public void OnSavePanelPlayPressed()
    {
        if (gameStarted) return;
        if (selectedSaveSlot < 1) return;

        GameManager.instance.LoadGame(selectedSaveSlot);

        StartCoroutine(DiveTransition());
    }

    void RefreshSlotHighlights()
    {
        SetButtonColor(slot1Button, selectedSaveSlot == 1 ? selectedSlotColor : normalSlotColor);
        SetButtonColor(slot2Button, selectedSaveSlot == 2 ? selectedSlotColor : normalSlotColor);
        SetButtonColor(slot3Button, selectedSaveSlot == 3 ? selectedSlotColor : normalSlotColor);
    }

    void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;

        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        colors.highlightedColor = color;
        button.colors = colors;
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("Fade Image is not assigned!");
            yield break;
        }

        fadeImage.gameObject.SetActive(true);
        fadeImage.transform.SetAsLastSibling();

        Color c = fadeImage.color;
        c.r = 0f;
        c.g = 0f;
        c.b = 0f;
        c.a = 0f;
        fadeImage.color = c;

        float t = 0f;

        while (t < fadeToBlackDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeToBlackDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = Color.black;
    }
}
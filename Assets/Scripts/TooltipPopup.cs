using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TooltipPopup : MonoBehaviour
{
    public static TooltipPopup Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button closeButton;
    public Toggle tooltipToggle;

    [Header("Settings")]
    public float fadeDuration = 0.15f;
    public float autoCloseTime = 3f;
    public bool tooltipsEnabled = false;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private Coroutine autoCloseCoroutine;

    private const string PrefsEnabledKey = "tooltips_enabled";
    private static HashSet<string> shownKeys = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetSessionState()
    {
        shownKeys.Clear();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        tooltipsEnabled = PlayerPrefs.GetInt(PrefsEnabledKey, 0) == 1;
        CacheSceneReferences();
        ApplyToggleState();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Close();
        CacheSceneReferences();
        ApplyToggleState();
    }

    void CacheSceneReferences()
    {
        if (panel == null)
            panel = GameObject.Find("TooltipPanel");

        if (panel != null)
        {
            if (canvasGroup == null)
                canvasGroup = panel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = panel.AddComponent<CanvasGroup>();

            panel.SetActive(false);
            canvasGroup.alpha = 0f;

            if (titleText == null)
            {
                Transform t = panel.transform.Find("TitleText");
                if (t != null) titleText = t.GetComponent<TextMeshProUGUI>();
            }

            if (bodyText == null)
            {
                Transform t = panel.transform.Find("BodyText");
                if (t != null) bodyText = t.GetComponent<TextMeshProUGUI>();
            }

            if (closeButton == null)
            {
                Transform t = panel.transform.Find("CloseButton");
                if (t != null) closeButton = t.GetComponent<Button>();
            }
        }

        Toggle[] toggles = FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in toggles)
        {
            if (t.name == "TooltipToggle")
            {
                tooltipToggle = t;
                break;
            }
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        if (tooltipToggle != null)
        {
            tooltipToggle.onValueChanged.RemoveAllListeners();
            tooltipToggle.SetIsOnWithoutNotify(tooltipsEnabled);
            tooltipToggle.onValueChanged.AddListener(SetTooltipsEnabled);
        }
    }

    void ApplyToggleState()
    {
        if (tooltipToggle != null)
            tooltipToggle.SetIsOnWithoutNotify(tooltipsEnabled);
    }

    public bool IsOpen => panel != null && panel.activeSelf;

    public void Show(string title, string body)
    {
        if (!tooltipsEnabled) return;
        if (panel == null || titleText == null || bodyText == null) return;

        titleText.text = title;
        bodyText.text = body;

        panel.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn());

        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);
        autoCloseCoroutine = StartCoroutine(AutoClose());
    }

    public void ShowOnce(string key, string title, string body)
    {
        if (!tooltipsEnabled) return;
        if (string.IsNullOrEmpty(key)) return;
        if (shownKeys.Contains(key)) return;

        shownKeys.Add(key);
        Show(title, body);
    }

    public void Close()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (panel != null)
            panel.SetActive(false);
    }

    public void SetTooltipsEnabled(bool enabled)
    {
        tooltipsEnabled = enabled;
        PlayerPrefs.SetInt(PrefsEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (tooltipToggle != null)
            tooltipToggle.SetIsOnWithoutNotify(tooltipsEnabled);
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null)
            yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = elapsed / fadeDuration;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        fadeCoroutine = null;
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSecondsRealtime(autoCloseTime);
        Close();
    }
}
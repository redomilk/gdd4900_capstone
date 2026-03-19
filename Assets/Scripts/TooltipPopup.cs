using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipPopup : MonoBehaviour
{
    public static TooltipPopup Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button closeButton;
    public Toggle tooltipToggle; // assign your Tooltip Toggle in Inspector

    [Header("Settings")]
    public float fadeDuration = 0.15f;
    public bool tooltipsEnabled = false;

    private CanvasGroup _cg;
    private Coroutine _fade;
    private const string PrefsKey = "tooltips_enabled";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _cg = panel.GetComponent<CanvasGroup>();
        if (_cg == null) _cg = panel.AddComponent<CanvasGroup>();
        closeButton.onClick.AddListener(Close);
        panel.SetActive(false);

        tooltipsEnabled = PlayerPrefs.GetInt(PrefsKey, 0) == 1;

        // Sync toggle visual to match saved state without firing OnValueChanged
        if (tooltipToggle != null)
            tooltipToggle.SetIsOnWithoutNotify(tooltipsEnabled);
    }

    public bool IsOpen => panel.activeSelf;

    public void Show(string title, string body)
    {
        if (!tooltipsEnabled) return;
        titleText.text = title;
        bodyText.text = body;
        panel.SetActive(true);
        Time.timeScale = 0f;
        if (_fade != null) StopCoroutine(_fade);
        _fade = StartCoroutine(FadeIn());
    }

    public void ShowOnce(string key, string title, string body)
    {
        if (!tooltipsEnabled) return;
        if (PlayerPrefs.GetInt(key, 0) == 1) return;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Show(title, body);
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SetTooltipsEnabled(bool enabled)
    {
        tooltipsEnabled = enabled;
        PlayerPrefs.SetInt(PrefsKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private IEnumerator FadeIn()
    {
        _cg.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            _cg.alpha = elapsed / fadeDuration;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        _cg.alpha = 1f;
    }
}
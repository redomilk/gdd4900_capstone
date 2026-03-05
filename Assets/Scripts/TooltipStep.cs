using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────────────────────
//  TooltipPopup
//  - Attach to your Tooltip Canvas
//  - Call TooltipPopup.Instance.Show("Title", "Body text") from any script
//  - Game freezes while tooltip is open
//  - Player closes it with the OK / X button
// ─────────────────────────────────────────────────────────────────────────────
public class TooltipPopup : MonoBehaviour
{
    public static TooltipPopup Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button closeButton;

    [Header("Settings")]
    public float fadeDuration = 0.15f;

    private CanvasGroup _cg;
    private Coroutine _fade;

    // ── Setup ─────────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _cg = panel.GetComponent<CanvasGroup>();
        if (_cg == null) _cg = panel.AddComponent<CanvasGroup>();

        closeButton.onClick.AddListener(Close);
        panel.SetActive(false);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Freeze the game and show a tooltip popup.
    /// Call this from your depth-trigger scripts.
    ///
    /// Example:
    ///     TooltipPopup.Instance.Show("Welcome!", "Use WASD to move around.");
    /// </summary>
    ///
    public bool IsOpen => panel.activeSelf;

    public void Show(string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;

        panel.SetActive(true);
        Time.timeScale = 0f;        // freeze everything

        if (_fade != null) StopCoroutine(_fade);
        _fade = StartCoroutine(FadeIn());
    }

    /// <summary>Close the tooltip and unfreeze the game.</summary>
    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;        // unfreeze
    }

    // ── Fade ──────────────────────────────────────────────────────────────────
    private IEnumerator FadeIn()
    {
        _cg.alpha = 0f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            _cg.alpha = elapsed / fadeDuration;
            elapsed += Time.unscaledDeltaTime;    // unscaled so it works while frozen
            yield return null;
        }
        _cg.alpha = 1f;
    }
}
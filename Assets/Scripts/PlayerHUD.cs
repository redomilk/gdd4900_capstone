using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;

    [Header("UI Elements")]
    public Image healthFill;
    public Image oxygenFill;
    public TextMeshProUGUI promptText;

    [Header("Prompt Settings")]
    public Vector2 promptOffset = new Vector2(0f, 80f);

    GameObject player;
    private bool _oxygenTipFired;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");

        if (promptText != null)
        {
            // center the pivot so text is centered on the position
            promptText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            promptText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            promptText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            promptText.alignment = TextAlignmentOptions.Center;
            promptText.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (promptText != null && promptText.gameObject.activeSelf && player != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);

            // convert screen position to canvas position properly
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                promptText.rectTransform.parent as RectTransform,
                screenPos,
                null,
                out Vector2 localPoint
            );

            promptText.rectTransform.localPosition = localPoint + promptOffset;
            promptText.rectTransform.rotation = Quaternion.identity;
        }
    }

    void OnEnable()
    {
        GameEvents.OnHealthChanged += HandleHealthChanged;
        GameEvents.OnOxygenChanged += HandleOxygenChanged;
        GameEvents.OnPlayerDied += HandlePlayerDied;
    }

    void OnDisable()
    {
        GameEvents.OnHealthChanged -= HandleHealthChanged;
        GameEvents.OnOxygenChanged -= HandleOxygenChanged;
        GameEvents.OnPlayerDied -= HandlePlayerDied;
    }

    void HandleHealthChanged(float current, float max)
    {
        if (healthFill) healthFill.fillAmount = (max <= 0f) ? 0f : current / max;
    }

    void HandleOxygenChanged(float current, float max)
    {
        if (oxygenFill) oxygenFill.fillAmount = (max <= 0f) ? 0f : current / max;

        // Fire once when oxygen drops below 25%
        if (!_oxygenTipFired && max > 0f && (current / max) < 0.25f)
        {
            _oxygenTipFired = true;
            TooltipPopup.Instance?.ShowOnce("tip_oxygen", "Low Oxygen!",
                "Oxygen is running low.\n\n" +
                "You will take fixed damaged over time when oxygen is empty \n\n" +
                "Find airpockets or kill enemies for bubbles.\n\n");
        }
    }

    void HandlePlayerDied()
    {
        if (GameManager.instance != null)
            GameManager.instance.DeathScrapPenalty(); //take some scrap away for dying

        SceneManager.LoadScene("HUB");
    }

    public void ShowPrompt(string message)
    {
        if (promptText == null) return;
        promptText.text = message;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }
}
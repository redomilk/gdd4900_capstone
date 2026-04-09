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

    public TextMeshProUGUI healthText; //value text
    public TextMeshProUGUI oxygenText; //value text

    public TextMeshProUGUI promptText;

    [Header("Prompt Settings")]
    public Vector2 promptOffset = new Vector2(0f, 80f);

    [Header("Shake Settings")]
    public float shakeAmount = 5f;
    public float shakeSpeed = 25f;

    private Vector3 healthTextOriginalPos;
    private Vector3 oxygenTextOriginalPos;

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

        //store hp/o2 vallue text position
        if (healthText)
            healthTextOriginalPos = healthText.rectTransform.localPosition;

        if (oxygenText)
            oxygenTextOriginalPos = oxygenText.rectTransform.localPosition;
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

    void Update()
    {
        ShakeText(healthText, healthFill.fillAmount, healthTextOriginalPos);
        ShakeText(oxygenText, oxygenFill.fillAmount, oxygenTextOriginalPos);
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
        if (healthFill)
            healthFill.fillAmount = (max <= 0f) ? 0f : current / max;

        //update hud text
        if (healthText)
            healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }

    void HandleOxygenChanged(float current, float max)
    {
        if (oxygenFill) oxygenFill.fillAmount = (max <= 0f) ? 0f : current / max;

        //update hud text
        if (oxygenText)
            oxygenText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";

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

    void ShakeText(TextMeshProUGUI text, float percent, Vector3 originalPos)
    {
        if (text == null) return;

        // Only shake when below 25%
        if (percent < 0.25f)
        {
            float strength = Mathf.Lerp(0f, shakeAmount, 1f - percent);

            Vector2 offset = Random.insideUnitCircle * strength;

            text.rectTransform.localPosition = originalPos + (Vector3)offset;
        }
        else
        {
            // Reset position when not low
            text.rectTransform.localPosition = originalPos;
        }
    }
}
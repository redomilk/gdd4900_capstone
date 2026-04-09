using UnityEngine;
using UnityEngine.UI;

public class OxygenVignetteEffect : MonoBehaviour
{
    [Header("Settings")]
    public float lowOxygenThreshold = 30f;  // % at which vignette starts appearing
    public float maxAlpha = 0.85f;
    public float pulseSpeed = 3f;
    public float fadeSmoothness = 3f;

    Image vignetteImage;
    float targetAlpha = 0f;

    void Awake()
    {
        vignetteImage = GetComponent<Image>();
    }

    void OnEnable()
    {
        GameEvents.OnOxygenChanged += HandleOxygenChanged;
    }

    void OnDisable()
    {
        GameEvents.OnOxygenChanged -= HandleOxygenChanged;
    }

    void HandleOxygenChanged(float current, float max)
    {
        float oxygenPercent = current / max * 100f;

        if (oxygenPercent < lowOxygenThreshold)
        {
            // 0 at threshold, 1 at zero oxygen
            float t = 1f - (oxygenPercent / lowOxygenThreshold);
            targetAlpha = t * maxAlpha;
        }
        else
        {
            targetAlpha = 0f;
        }
    }

    void Update()
    {
        float pulse = 0.85f + 0.15f * Mathf.Sin(Time.time * pulseSpeed);
        float pulsedTarget = targetAlpha * pulse;

        Color c = vignetteImage.color;
        c.a = Mathf.Lerp(c.a, pulsedTarget, Time.deltaTime * fadeSmoothness);
        vignetteImage.color = c;
    }
}
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class OptionsMenu : MonoBehaviour
{
    [Header("Sliders")]
    public Slider darknessSlider;
    public Slider volumeSlider;

    [Header("References")]
    public DepthLighting depthLighting;

    FMOD.Studio.Bus masterBus;

    void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");

        // Hook up listeners in code instead of Inspector
        darknessSlider.onValueChanged.AddListener(OnDarknessChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnEnable()
    {
        // Temporarily remove listeners so setting .value doesn't trigger callbacks
        darknessSlider.onValueChanged.RemoveListener(OnDarknessChanged);
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

        darknessSlider.value = depthLighting != null ? depthLighting.darknessStrength : 1f;

        masterBus.getVolume(out float vol);
        volumeSlider.value = vol;

        // Re-add listeners after values are set
        darknessSlider.onValueChanged.AddListener(OnDarknessChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnDarknessChanged(float value)
    {
        if (depthLighting != null)
            depthLighting.darknessStrength = value;
    }

    public void OnVolumeChanged(float value)
    {
        masterBus.setVolume(value);
    }
}
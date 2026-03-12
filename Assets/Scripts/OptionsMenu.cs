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
        depthLighting = FindFirstObjectByType<DepthLighting>();

        darknessSlider.onValueChanged.RemoveListener(OnDarknessChanged);
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

        darknessSlider.value = depthLighting != null ? depthLighting.darknessStrength : 1f;
        masterBus.getVolume(out float vol);
        volumeSlider.value = vol;

        darknessSlider.onValueChanged.AddListener(OnDarknessChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnDarknessChanged(float value)
    {
        if (depthLighting == null)
            depthLighting = FindFirstObjectByType<DepthLighting>();
        if (depthLighting != null)
            depthLighting.darknessStrength = value;
    }

    public void OnVolumeChanged(float value)
    {
        masterBus.setVolume(value);
    }
}
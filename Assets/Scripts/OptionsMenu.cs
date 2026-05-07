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
        GetMasterBusIfNeeded();
    }

    void OnEnable()
    {
        GetMasterBusIfNeeded();

        if (depthLighting == null)
            depthLighting = FindFirstObjectByType<DepthLighting>();

        if (darknessSlider != null)
            darknessSlider.onValueChanged.RemoveListener(OnDarknessChanged);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

        float savedDarkness = PlayerPrefs.GetFloat("DarknessStrength", 1f);
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        if (darknessSlider != null)
            darknessSlider.value = depthLighting != null ? depthLighting.darknessStrength : savedDarkness;

        if (volumeSlider != null)
        {
            masterBus.getVolume(out float vol);
            volumeSlider.value = vol;
        }

        if (darknessSlider != null)
            darknessSlider.onValueChanged.AddListener(OnDarknessChanged);

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnVolumeChanged(float value)
    {
        GetMasterBusIfNeeded();

        masterBus.setVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void OnDarknessChanged(float value)
    {
        PlayerPrefs.SetFloat("DarknessStrength", value);
        PlayerPrefs.Save();

        if (depthLighting == null)
            depthLighting = FindFirstObjectByType<DepthLighting>();

        if (depthLighting != null)
            depthLighting.darknessStrength = value;
    }

    void GetMasterBusIfNeeded()
    {
        if (!masterBus.hasHandle())
            masterBus = RuntimeManager.GetBus("bus:/");
    }
}
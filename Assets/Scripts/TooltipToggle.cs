using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TooltipToggle : MonoBehaviour
{
    private const string PrefsEnabledKey = "tooltips_enabled";

    Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void OnEnable()
    {
        bool savedValue = PlayerPrefs.GetInt(PrefsEnabledKey, 0) == 1;

        toggle.onValueChanged.RemoveListener(OnToggleChanged);

        toggle.SetIsOnWithoutNotify(savedValue);

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool value)
    {
        PlayerPrefs.SetInt(PrefsEnabledKey, value ? 1 : 0);
        PlayerPrefs.Save();

        if (TooltipPopup.Instance != null)
        {
            TooltipPopup.Instance.SetTooltipsEnabled(value);

            // retrigger all ShowOnce tooltips
            TooltipPopup.ResetShownTips();
        }
    }
}
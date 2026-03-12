using UnityEngine;
using UnityEngine.UI;

// Attach to the same GameObject as your Toggle checkbox in the options menu.
// Drag the Toggle component into the toggle field in the Inspector.
[RequireComponent(typeof(Toggle))]
public class TooltipToggle : MonoBehaviour
{
    Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Start()
    {
        // Set the checkbox to match the saved preference on open
        if (TooltipPopup.Instance != null)
            toggle.isOn = TooltipPopup.Instance.tooltipsEnabled;

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool value)
    {
        if (TooltipPopup.Instance != null)
            TooltipPopup.Instance.SetTooltipsEnabled(value);
    }
}
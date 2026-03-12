using UnityEngine;
using UnityEngine.UI;

public class DepthGauge : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform depthMarker;
    public Slider depthSlider;

    float startingDepth;
    bool _objectiveTipFired;
    bool _movementTipFired;
    bool _flashlightTipFired;

    void Start()
    {
        startingDepth = Mathf.Abs(player.position.y - depthMarker.position.y);
        depthSlider.value = 0f;

        // Reset tip keys so they fire again each new session
        PlayerPrefs.DeleteKey("tip_objective");
        PlayerPrefs.DeleteKey("tip_movement");
        PlayerPrefs.DeleteKey("tip_flashlight");
        PlayerPrefs.Save();
    }

    void Update()
    {
        float currentDepth = Mathf.Abs(player.position.y - depthMarker.position.y);
        float normalized = 1f - Mathf.Clamp01(currentDepth / startingDepth);
        depthSlider.value = normalized;

        if (TooltipPopup.Instance == null) return;
        if (TooltipPopup.Instance.IsOpen) return;

        if (!_objectiveTipFired && player.position.y <= 5f)
        {
            _objectiveTipFired = true;
            TooltipPopup.Instance.ShowOnce("tip_objective", "Objective",
                "Defeat enemies and collect scrap to buy upgrades.\n\n" +
                "Dive deeper into the abyss.\n\n" +
                "Escape at an extraction point to return with full scrap value.");
            return;
        }

        if (!_movementTipFired && player.position.y <= 3f)
        {
            _movementTipFired = true;
            TooltipPopup.Instance.ShowOnce("tip_movement", "Movement & Controls",
                "W A S D to move.\n\n" +
                "Spacebar to boost.\n\n" +
                "Left click to fire ranged.\n\n" +
                "Right click to melee. \n\n" +
                "<color=red>Melee Reflects enemy bullets</color>");
            return;
        }

        if (!_flashlightTipFired && player.position.y <= -45f)
        {
            _flashlightTipFired = true;
            TooltipPopup.Instance.ShowOnce("tip_flashlight", "Flashlight",
                "The depths are drenched in darkness.\n\n" +
                "Press F to toggle your flashlight.");
        }
    }
}
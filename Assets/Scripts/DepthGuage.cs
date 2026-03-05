using UnityEngine;
using UnityEngine.UI;

public class DepthGauge : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform depthMarker;
    public Slider depthSlider;

    float startingDepth;
    private bool _objectiveTipFired;
    private bool _movementTipFired;
    private bool _flashlightTipFired;

    void Start()
    {
        startingDepth = Mathf.Abs(player.position.y - depthMarker.position.y);
        depthSlider.value = 0f;
    }

    void Update()
    {
        float currentDepth = Mathf.Abs(player.position.y - depthMarker.position.y);
        float normalized = 1f - Mathf.Clamp01(currentDepth / startingDepth);
        depthSlider.value = normalized;

        // Don't check triggers while a tooltip is already open
        if (TooltipPopup.Instance.IsOpen) return;

        // Objective tip at Y = 5
        if (!_objectiveTipFired && player.position.y <= 5f)
        {
            _objectiveTipFired = true;
            TooltipPopup.Instance.Show("Objective",
                "Defeat enemies and collect scrap to buy upgrades.\n\n" +
                "Dive deeper into the abyss.\n\n" +
                "Escape at an extraction point to return with full scrap value.");
            return; // wait until this one is closed before checking the next
        }

        // Movement tip at Y = 3
        if (!_movementTipFired && player.position.y <= 3f)
        {
            _movementTipFired = true;
            TooltipPopup.Instance.Show("Movement & Controls",
                "W A S D to move.\n\n" +
                "Spacebar to boost.\n\n +" +
                "Left click to fire ranged \n\n +" +
                "Right click to melee \n\n");
        }

        // Flashlight tip at Y = -45
        if (!_flashlightTipFired && player.position.y <= -45f)
        {
            _flashlightTipFired = true;
            TooltipPopup.Instance.Show("Flashlight",
                "The depths are drenched in darkness - use flashlight to navigate the depth.\n\n" +
                "F to toggle/untoggle flashlight\n\n");
        }
    }
}
using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  DepthTooltipTrigger2D
//  - Attach to your Player object
//  - Add entries in the Inspector — each fires once when the player's Y
//    drops to or below that value
// ─────────────────────────────────────────────────────────────────────────────
public class TooltipTrigger : MonoBehaviour
{
    [System.Serializable]
    public class DepthTrigger
    {
        public string title = "Tip Title";

        [TextArea(2, 4)]
        public string body = "Tip description.";

        [Tooltip("Fires when the player's Y position reaches this value.\n" +
                 "Lower numbers = deeper. e.g. -5, -20, -50")]
        public float triggerY = 0f;

        [HideInInspector]
        public bool fired = false;
    }

    [Header("Depth Triggers  (order doesn't matter)")]
    public DepthTrigger[] triggers;

    private void Update()
    {
        float y = transform.position.y;

        foreach (DepthTrigger t in triggers)
        {
            if (!t.fired && y <= t.triggerY)
            {
                t.fired = true;
                TooltipPopup.Instance.Show(t.title, t.body);
            }
        }
    }

    /// <summary>Reset all triggers — useful on scene reload or respawn.</summary>
    public void ResetTriggers()
    {
        foreach (DepthTrigger t in triggers)
            t.fired = false;
    }
}
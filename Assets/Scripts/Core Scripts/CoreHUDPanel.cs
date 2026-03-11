using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// Attach to a Canvas GameObject that acts as the Core HUD overlay.
/// Wire up the four CoreSlotUI references in the Inspector.
public class CoreHUDPanel : MonoBehaviour
{
    public static CoreHUDPanel instance;

    [Header("Panel Root")]
    public GameObject panelRoot;       // The full overlay — toggled by Tab

    [Header("Slot UI References")]
    public CoreSlotUI mainSlotUI;
    public CoreSlotUI meleeSlotUI;
    public CoreSlotUI rangedSlotUI;
    public CoreSlotUI boosterSlotUI;

    bool isOpen = false;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    void Start()
    {
        panelRoot.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
            Toggle();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        panelRoot.SetActive(isOpen);

        if (isOpen)
            RefreshAll();
    }

    public void RefreshAll()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        CoreInventory inv = player.GetComponent<CoreInventory>();
        if (inv == null) return;

        mainSlotUI?.Refresh(inv.mainCore, "MAIN CORE");
        meleeSlotUI?.Refresh(inv.meleeCore, "MELEE CORE");
        rangedSlotUI?.Refresh(inv.rangedCore, "RANGED CORE");
        boosterSlotUI?.Refresh(inv.boosterCore, "BOOSTER CORE");
    }
}

//--------------Per-slot UI component-------------------

[System.Serializable]
public class CoreSlotUI
{
    [Header("References")]
    public Image iconImage;
    public TextMeshProUGUI slotLabel;
    public TextMeshProUGUI coreNameText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI effectText;
    public Image rarityBorder;       // Optional coloured border image
    public GameObject emptyOverlay;       // Shown when slot is empty

    public void Refresh(CoreData core, string slotName)
    {
        if (slotLabel != null)
            slotLabel.text = slotName;

        bool isEmpty = core == null;

        if (emptyOverlay != null)
            emptyOverlay.SetActive(isEmpty);

        if (isEmpty)
        {
            if (coreNameText != null) coreNameText.text = "— Empty —";
            if (rarityText != null) rarityText.text = "";
            if (statsText != null) statsText.text = "";
            if (effectText != null) effectText.text = "";
            if (iconImage != null) iconImage.sprite = null;
            if (rarityBorder != null) rarityBorder.color = Color.grey;
            return;
        }

        // Icon
        if (iconImage != null)
        {
            iconImage.sprite = core.icon;
            iconImage.enabled = core.icon != null;
        }

        // Name
        if (coreNameText != null)
            coreNameText.text = core.coreName;

        // Rarity
        Color rc = core.RarityColor();
        if (rarityText != null)
        {
            rarityText.text = core.RarityLabel();
            rarityText.color = rc;
        }
        if (rarityBorder != null)
            rarityBorder.color = rc;

        // Stats block
        if (statsText != null)
            statsText.text = BuildStatsString(core);

        // Effect block
        if (effectText != null)
            effectText.text = BuildEffectString(core);
    }

    // -----------Helpers -------------

    string BuildStatsString(CoreData c)
    {
        var sb = new System.Text.StringBuilder();

        void AddStat(string label, float val)
        {
            if (Mathf.Abs(val) < 0.001f) return;
            sb.AppendLine($"{label}: {(val >= 0 ? "+" : "")}{val:0.##}");
        }

        AddStat("Health", c.healthBonus);
        AddStat("Oxygen", c.oxygenBonus);
        AddStat("Speed", c.speedBonus);
        AddStat("Damage", c.damageBonus);
        AddStat("Defense", c.defenseBonus);
        AddStat("Atk Speed", c.attackSpeedBonus);

        if (c.slot == CoreSlot.Booster)
        {
            if (c.dashCooldownReduction > 0f)
                sb.AppendLine($"Dash CD: -{c.dashCooldownReduction:0.##}s");
            if (Mathf.Abs(c.dashSpeedMultiplier - 1f) > 0.001f)
                sb.AppendLine($"Dash Speed: x{c.dashSpeedMultiplier:0.##}");
        }

        return sb.Length > 0 ? sb.ToString().TrimEnd() : "No stat bonuses";
    }

    string BuildEffectString(CoreData c)
    {
        var sb = new System.Text.StringBuilder();

        if (c.slot == CoreSlot.Melee && c.meleeEffect != MeleeEffect.None)
        {
            sb.Append($"Effect: {c.meleeEffect}");
            switch (c.meleeEffect)
            {
                case MeleeEffect.Knockback: sb.Append($" (Force: {c.knockbackForce:0.#})"); break;
                case MeleeEffect.Bleed: sb.Append($" ({c.bleedDPS:0.#} DPS)"); break;
                case MeleeEffect.Stun: sb.Append($" ({c.stunDuration:0.#}s)"); break;
                case MeleeEffect.Lifesteal: sb.Append($" ({c.lifestealPercent * 100f:0}% steal)"); break;
            }
        }

        if (c.slot == CoreSlot.Ranged && c.rangedEffect != RangedEffect.None)
        {
            sb.Append($"Effect: {c.rangedEffect}");
            switch (c.rangedEffect)
            {
                case RangedEffect.Freeze: sb.Append($" ({c.freezeDuration:0.#}s, {c.freezeSlowPercent * 100f:0}% slow)"); break;
                case RangedEffect.Explosive: sb.Append($" (r:{c.explosionRadius:0.#}, {c.explosionDamage:0.#} dmg)"); break;
                case RangedEffect.Chain: sb.Append($" (x{c.chainTargets} targets)"); break;
            }
        }

        return sb.Length > 0 ? sb.ToString() : "";
    }
}
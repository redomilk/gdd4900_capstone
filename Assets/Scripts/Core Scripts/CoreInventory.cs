using UnityEngine;

/// Manages the player's 4 equipped cores and applies their stat/effect bonuses.
public class CoreInventory : MonoBehaviour
{
    [Header("Equipped Cores")]
    public CoreData mainCore;
    public CoreData meleeCore;
    public CoreData rangedCore;
    public CoreData boosterCore;

    [Header("Drop Settings")]
    public GameObject corePickupPrefab;

    //--------- Public slot access --------------------------

    public CoreData GetSlot(CoreSlot slot) => slot switch
    {
        CoreSlot.Main => mainCore,
        CoreSlot.Melee => meleeCore,
        CoreSlot.Ranged => rangedCore,
        CoreSlot.Booster => boosterCore,
        _ => null
    };

    private void SetSlot(CoreSlot slot, CoreData data)
    {
        switch (slot)
        {
            case CoreSlot.Main: mainCore = data; break;
            case CoreSlot.Melee: meleeCore = data; break;
            case CoreSlot.Ranged: rangedCore = data; break;
            case CoreSlot.Booster: boosterCore = data; break;
        }
    }

    /// Equips newCore into its slot. Returns the previously equipped core (may be null).
    public CoreData Equip(CoreData newCore)
    {
        CoreData previous = GetSlot(newCore.slot);

        // Remove old stat contribution before swapping
        if (previous != null)
            RemoveCoreStats(previous);

        SetSlot(newCore.slot, newCore);
        ApplyCoreStats(newCore);

        // Notify HUD
        CoreHUDPanel.instance?.RefreshAll();

        return previous;
    }

    // ------------- Stat application ---------------------

    void ApplyCoreStats(CoreData core)
    {
        if (core == null) return;

        PlayerStats ps = GetComponent<PlayerStats>();
        PlayerDiveController dc = GetComponent<PlayerDiveController>();

        if (ps != null)
        {
            ps.maxHealth += core.healthBonus;
            ps.health = Mathf.Min(ps.health + core.healthBonus, ps.maxHealth);
            ps.maxOxygen += core.oxygenBonus;
            GameEvents.OnHealthChanged?.Invoke(ps.health, ps.maxHealth);
            GameEvents.OnOxygenChanged?.Invoke(ps.oxygen, ps.maxOxygen);
        }

        if (dc != null)
        {
            dc.maxHorizontalSpeed += core.speedBonus;
            dc.maxVerticalSpeed += core.speedBonus;

            // Booster-specific
            if (core.slot == CoreSlot.Booster)
            {
                dc.maxHorizontalSpeed *= core.dashSpeedMultiplier;
                dc.maxVerticalSpeed *= core.dashSpeedMultiplier;
            }
        }

        // Combat stats — forward to CoreEffects component
        CoreEffects ce = GetComponent<CoreEffects>();
        if (ce != null)
            ce.RecalculateFromInventory(this);
    }

    void RemoveCoreStats(CoreData core)
    {
        if (core == null) return;

        PlayerStats ps = GetComponent<PlayerStats>();
        PlayerDiveController dc = GetComponent<PlayerDiveController>();

        if (ps != null)
        {
            ps.maxHealth = Mathf.Max(1f, ps.maxHealth - core.healthBonus);
            ps.health = Mathf.Min(ps.health, ps.maxHealth);
            ps.maxOxygen = Mathf.Max(0f, ps.maxOxygen - core.oxygenBonus);
            GameEvents.OnHealthChanged?.Invoke(ps.health, ps.maxHealth);
            GameEvents.OnOxygenChanged?.Invoke(ps.oxygen, ps.maxOxygen);
        }

        if (dc != null)
        {
            dc.maxHorizontalSpeed -= core.speedBonus;
            dc.maxVerticalSpeed -= core.speedBonus;

            if (core.slot == CoreSlot.Booster && core.dashSpeedMultiplier != 0f)
            {
                dc.maxHorizontalSpeed /= core.dashSpeedMultiplier;
                dc.maxVerticalSpeed /= core.dashSpeedMultiplier;
            }
        }

        CoreEffects ce = GetComponent<CoreEffects>();
        if (ce != null)
            ce.RecalculateFromInventory(this);
    }

    //---------Convenience sums (used by CoreEffects & damage systems)---------

    public float TotalDamageBonus()
    {
        float sum = 0f;
        foreach (var slot in System.Enum.GetValues(typeof(CoreSlot)))
        {
            CoreData c = GetSlot((CoreSlot)slot);
            if (c != null) sum += c.damageBonus;
        }
        return sum;
    }

    public float TotalAttackSpeedBonus()
    {
        float sum = 0f;
        foreach (var slot in System.Enum.GetValues(typeof(CoreSlot)))
        {
            CoreData c = GetSlot((CoreSlot)slot);
            if (c != null) sum += c.attackSpeedBonus;
        }
        return sum;
    }
}
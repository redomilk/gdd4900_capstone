using UnityEngine;

public class AugmentInventory : MonoBehaviour
{
    public AugmentData coreAugment;
    public AugmentData armsAugment;
    public AugmentData legsAugment;

    [Header("Drop Settings")]
    public GameObject augmentPickupPrefab;  // prefab that holds AugmentPickup.cs

    public AugmentData GetSlot(AugmentSlot slot)
    {
        return slot switch
        {
            AugmentSlot.Core => coreAugment,
            AugmentSlot.Arms => armsAugment,
            AugmentSlot.Legs => legsAugment,
            _ => null
        };
    }

    public AugmentData Equip(AugmentData newAugment)
    {
        AugmentData previous = GetSlot(newAugment.slot);

        // Put new augment in slot
        switch (newAugment.slot)
        {
            case AugmentSlot.Core: coreAugment = newAugment; break;
            case AugmentSlot.Arms: armsAugment = newAugment; break;
            case AugmentSlot.Legs: legsAugment = newAugment; break;
        }

        // Apply stats
        ApplyAugment(newAugment);

        // Return old one so it can be dropped
        return previous;
    }

    void ApplyAugment(AugmentData augment)
    {
        PlayerStats ps = GetComponent<PlayerStats>();
        PlayerDiveController dc = GetComponent<PlayerDiveController>();

        if (ps != null)
        {
            ps.maxHealth += augment.healthBonus;
            ps.health = Mathf.Min(ps.health + augment.healthBonus, ps.maxHealth);
            ps.maxOxygen += augment.oxygenBonus;
            GameEvents.OnHealthChanged?.Invoke(ps.health, ps.maxHealth);
            GameEvents.OnOxygenChanged?.Invoke(ps.oxygen, ps.maxOxygen);
        }

        if (dc != null)
        {
            dc.maxHorizontalSpeed += augment.speedBonus;
            dc.maxVerticalSpeed += augment.speedBonus;
        }
    }
}
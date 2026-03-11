using UnityEngine;

public enum CoreSlot { Main, Melee, Ranged, Booster }

public enum CoreRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    UltraRare
}

public enum MeleeEffect { None, Knockback, Bleed, Stun, Lifesteal }
public enum RangedEffect { None, Freeze, Pierce, Explosive, Chain }

[CreateAssetMenu(fileName = "CoreData", menuName = "Cores/CoreData")]
public class CoreData : ScriptableObject
{
    [Header("Identity")]
    public string coreName;
    public CoreSlot slot;
    public CoreRarity rarity;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Stat Modifiers")]
    public float healthBonus = 0f;
    public float oxygenBonus = 0f;
    public float speedBonus = 0f;
    public float damageBonus = 0f;
    public float defenseBonus = 0f;   // Main core specialty
    public float attackSpeedBonus = 0f;

    [Header("Melee Core Effects")]
    public MeleeEffect meleeEffect = MeleeEffect.None;
    public float knockbackForce = 0f;   // MeleeEffect.Knockback
    public float bleedDPS = 0f;   // MeleeEffect.Bleed
    public float stunDuration = 0f;   // MeleeEffect.Stun
    public float lifestealPercent = 0f;   // MeleeEffect.Lifesteal (0-1)

    [Header("Ranged Core Effects")]
    public RangedEffect rangedEffect = RangedEffect.None;
    public float freezeDuration = 0f;   // RangedEffect.Freeze
    public float freezeSlowPercent = 0.5f; // 0-1, how much to slow
    public float explosionRadius = 0f;   // RangedEffect.Explosive
    public float explosionDamage = 0f;
    public int chainTargets = 0;    // RangedEffect.Chain

    [Header("Booster Core Effects")]
    public float dashCooldownReduction = 0f;
    public float dashSpeedMultiplier = 1f;
    public float airControlBonus = 0f;

    //------Rarity Helpers----------

    /// Minimum depth (units) at which this rarity can spawn
    public static float MinDepthForRarity(CoreRarity r) => r switch
    {
        CoreRarity.Common => 0f,
        CoreRarity.Uncommon => 50f,
        CoreRarity.Rare => 120f,
        CoreRarity.Epic => 220f,
        CoreRarity.UltraRare => 350f,
        _ => 0f
    };

    public Color RarityColor() => rarity switch
    {
        CoreRarity.Common => new Color(0.75f, 0.75f, 0.75f),
        CoreRarity.Uncommon => new Color(0.18f, 0.85f, 0.18f),
        CoreRarity.Rare => new Color(0.18f, 0.55f, 1f),
        CoreRarity.Epic => new Color(0.65f, 0.18f, 1f),
        CoreRarity.UltraRare => new Color(1f, 0.55f, 0f),
        _ => Color.white
    };

    public string RarityLabel() => rarity switch
    {
        CoreRarity.Common => "Common",
        CoreRarity.Uncommon => "Uncommon",
        CoreRarity.Rare => "Rare",
        CoreRarity.Epic => "Epic",
        CoreRarity.UltraRare => "Ultra Rare",
        _ => "Unknown"
    };
}
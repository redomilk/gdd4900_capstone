using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health;

    [Header("Oxygen")]
    public float maxOxygen = 100f;
    public float oxygen;

    [Header("Resources")]
    public int scrapCount = 0;

    public float oxygenDrainPerSecond = 8f;       // while in water
    public float oxygenRegenPerSecond = 30f;      // while in air OR air pocket
    public float drowningDamagePerSecond = 1f;   // when oxygen == 0 while in water

    bool inWater;
    bool inAirPocket;

    void Awake()
    {
        health = maxHealth;
        oxygen = maxOxygen;
    }

    void OnEnable()
    {
        GameEvents.OnPlayerHitWater += OnEnterWater;
        GameEvents.OnPlayerLeftWater += OnExitWater;

        GameEvents.OnPlayerEnterAirPocket += OnEnterAirPocket;
        GameEvents.OnPlayerExitAirPocket += OnExitAirPocket;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerHitWater -= OnEnterWater;
        GameEvents.OnPlayerLeftWater -= OnExitWater;

        GameEvents.OnPlayerEnterAirPocket -= OnEnterAirPocket;
        GameEvents.OnPlayerExitAirPocket -= OnExitAirPocket;
    }

    void Start()
    {
        // Push initial UI state
        GameEvents.OnHealthChanged?.Invoke(health, maxHealth);
        GameEvents.OnOxygenChanged?.Invoke(oxygen, maxOxygen);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Air pocket overrides everything: always refill oxygen while inside
        if (inAirPocket)
        {
            if (oxygenRegenPerSecond > 0f)
                ModifyOxygen(+oxygenRegenPerSecond * dt);

            return;
        }

        if (inWater)
        {
            ModifyOxygen(-oxygenDrainPerSecond * dt);

            if (oxygen <= 0f)
                TakeDamage(drowningDamagePerSecond * dt);
        }
        else
        {
            if (oxygenRegenPerSecond > 0f)
                ModifyOxygen(+oxygenRegenPerSecond * dt);
        }
    }

    void OnEnterWater() => inWater = true;
    void OnExitWater() => inWater = false;

    void OnEnterAirPocket() => inAirPocket = true;
    void OnExitAirPocket() => inAirPocket = false;

    // --------- Public API (for enemies, hazards, augments later) ---------

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        health = Mathf.Max(0f, health - amount);
        GameEvents.OnHealthChanged?.Invoke(health, maxHealth);

        if (health <= 0f)
            GameEvents.OnPlayerDied?.Invoke();
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;

        health = Mathf.Min(maxHealth, health + amount);
        GameEvents.OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void ModifyOxygen(float amount)
    {
        if (amount == 0f) return;

        oxygen = Mathf.Clamp(oxygen + amount, 0f, maxOxygen);
        GameEvents.OnOxygenChanged?.Invoke(oxygen, maxOxygen);
    }

    public bool TrySpendOxygen(float cost)
    {
        if (cost <= 0f) return true;
        if (oxygen < cost) return false;

        ModifyOxygen(-cost);
        return true;
    }
}
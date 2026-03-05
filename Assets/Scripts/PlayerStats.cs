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
    public float oxygenDrainPerSecond = 8f;
    public float oxygenRegenPerSecond = 30f;
    public float drowningDamagePerSecond = 1f;

    [Header("Knockback")]
    public float knockbackForce = 6f;
    public float knockbackDuration = 0.15f;

    bool inWater;
    bool inAirPocket;

    Rigidbody2D rb;
    float knockbackTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Apply upgrades from GameManager if available
        if (GameManager.instance != null)
        {
            maxHealth = 100f + (GameManager.instance.healthLevel * GameManager.instance.hpPerLevel);
            maxOxygen = 100f + (GameManager.instance.oxygenLevel * GameManager.instance.oxygenPerLevel);
        }

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
        GameEvents.OnHealthChanged?.Invoke(health, maxHealth);
        GameEvents.OnOxygenChanged?.Invoke(oxygen, maxOxygen);
    }

    void Update()
    {
        float dt = Time.deltaTime;

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

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        health = Mathf.Max(0f, health - amount);
        GameEvents.OnHealthChanged?.Invoke(health, maxHealth);
        if (health <= 0f)
            GameEvents.OnPlayerDied?.Invoke();
    }

    public void TakeDamageWithKnockback(float amount, Vector2 sourcePosition)
    {
        TakeDamage(amount);

        if (rb == null) return;

        // Notify player movement to pause briefly
        PlayerDiveController pm = GetComponent<PlayerDiveController>();
        if (pm != null) pm.ApplyKnockback(knockbackDuration);

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
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
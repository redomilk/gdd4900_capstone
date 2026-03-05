using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public GameObject oxygenBubblePrefab;
    public GameObject scrapPrefab;
    public float maxHealth = 30f;
    public float health;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;

    Rigidbody2D rb;
    bool isKnockbackable;

    void Awake()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // Only apply knockback to Charger and Floater
        isKnockbackable = gameObject.name.Contains("EnemyCharger") ||
                          gameObject.name.Contains("EnemyFloater") ||
                          gameObject.name.Contains("EnemyExploder");
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        health = Mathf.Max(0f, health - amount);
        if (health <= 0f)
            Die();
    }

    public void TakeDamageWithKnockback(float amount, Vector2 sourcePosition, float force)  // CHANGED
    {
        TakeDamage(amount);
        if (!isKnockbackable || rb == null) return;

        FloaterController floater = GetComponent<FloaterController>();
        if (floater != null) floater.ApplyKnockback(knockbackDuration);

        PlayerFollow follower = GetComponent<PlayerFollow>();
        if (follower != null) follower.ApplyKnockback(knockbackDuration);

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);  // CHANGED
    }

    void Die()
    {
        GameObject drop = Random.value > 0.5f ? oxygenBubblePrefab : scrapPrefab;
        if (drop != null)
            Instantiate(drop, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
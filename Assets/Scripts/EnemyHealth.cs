using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IStunnable
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

    //------------ IStunnable-------------
    bool isStunned = false;
    float speedMultiplier = 1f;   // Used by movement scripts that read this component

    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    /// Movement scripts (FloaterController, PlayerFollow, etc.) should
    /// multiply their speed by this each frame to respect freeze/slow effects.
    public float GetSpeedMultiplier() => isStunned ? 0f : speedMultiplier;

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    //------------Lifecycle---------------

    void Awake()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        isKnockbackable = gameObject.name.Contains("EnemyCharger") ||
                          gameObject.name.Contains("EnemyFloater") ||
                          gameObject.name.Contains("EnemyExploder");
    }

    //-------------------IDamageable ------------------------

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        health = Mathf.Max(0f, health - amount);
        if (health <= 0f) Die();
    }

    public void TakeDamageWithKnockback(float amount, Vector2 sourcePosition, float force)
    {
        TakeDamage(amount);

        if (!isKnockbackable || rb == null || isStunned) return;

        FloaterController floater = GetComponent<FloaterController>();
        if (floater != null) floater.ApplyKnockback(knockbackDuration);

        PlayerFollow follower = GetComponent<PlayerFollow>();
        if (follower != null) follower.ApplyKnockback(knockbackDuration);

        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    //-----------------Death--------------------

    void Die()
    {
        GameObject drop = Random.value > 0.5f ? oxygenBubblePrefab : scrapPrefab;
        if (drop != null)
            Instantiate(drop, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public GameObject oxygenBubblePrefab;

    public float maxHealth = 30f;
    public float health;

    void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        health = Mathf.Max(0f, health - amount);
        Debug.Log($"{name} took {amount} damage. HP: {health}/{maxHealth}");

        if (health <= 0f)
            Die();
    }

    void Die()
    {
        // drop oxygen bubble
        if (oxygenBubblePrefab != null)
            Instantiate(oxygenBubblePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
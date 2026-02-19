using UnityEngine;
public class EnemyHealth : MonoBehaviour, IDamageable
{
    public GameObject oxygenBubblePrefab;
    public GameObject scrapPrefab;          // assign in Inspector
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
        if (health <= 0f)
            Die();
    }

    void Die()
    {
        // 50/50 drop
        GameObject drop = Random.value > 0.5f ? oxygenBubblePrefab : scrapPrefab;

        if (drop != null)
            Instantiate(drop, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
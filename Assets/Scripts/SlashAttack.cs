using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    public float lifetime = 0.12f;
    public float damage = 10f;
    public float knockback = 8f;
    public float reflectSpeedMultiplier = 1.5f;

    float timer;
    Vector2 slashDirection;
    Vector2 playerPosition;  // ADD THIS
    readonly HashSet<IDamageable> hitThisSwing = new HashSet<IDamageable>();

    void OnEnable()
    {
        timer = lifetime;
        hitThisSwing.Clear();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }

    public void Initialize(Vector2 direction)
    {
        slashDirection = direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetPlayerPosition(Vector2 position)  // ADD THIS
    {
        playerPosition = position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Reflect bullets
        if (other.CompareTag("Enemy Bullet"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = slashDirection * rb.linearVelocity.magnitude * reflectSpeedMultiplier;
                Bullet enemyScript = other.GetComponent<Bullet>();
                if (enemyScript != null) Destroy(enemyScript);
                PlayerBullet pb = other.gameObject.AddComponent<PlayerBullet>();
                pb.damage = damage;
                other.tag = "Player Bullet";
            }
            return;
        }

        if (!other.CompareTag("Enemy")) return;

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null) return;

        if (hitThisSwing.Add(dmg))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamageWithKnockback(damage, playerPosition, knockback);  // CHANGED
            else
                dmg.TakeDamage(damage);
        }
    }
}
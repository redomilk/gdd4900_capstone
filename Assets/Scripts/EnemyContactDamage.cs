using UnityEngine;
public class EnemyContactDamage : MonoBehaviour
{
    public float damagePerSecond = 10f;
    public float knockbackCooldown = 1f;

    PlayerStats playerInside;
    float knockbackTimer;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            playerInside = player;
            // Knockback once on enter, then cooldown prevents spam
            playerInside.TakeDamageWithKnockback(0f, transform.position);
            knockbackTimer = knockbackCooldown;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null && player == playerInside)
            playerInside = null;
    }

    void Update()
    {
        if (playerInside == null) return;

        // Damage every frame as normal
        playerInside.TakeDamage(damagePerSecond * Time.deltaTime);

        // Knockback only re-triggers after cooldown if still inside
        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0f)
        {
            playerInside.TakeDamageWithKnockback(0f, transform.position);
            knockbackTimer = knockbackCooldown;
        }
    }
}
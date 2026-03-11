using System.Collections;
using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public float damagePerSecond = 10f;
    public float knockbackCooldown = 1f;

    PlayerStats playerInside;
    float knockbackTimer;

    // -------------Update------------------

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            playerInside = player;
            playerInside.TakeDamageWithKnockback(0f, transform.position);
            knockbackTimer = knockbackCooldown;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
            playerInside = player;
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

        // Respect stun/slow from CoreEffects — read from EnemyHealth on same object
        EnemyHealth eh = GetComponent<EnemyHealth>();
        float speedMult = eh != null ? eh.GetSpeedMultiplier() : 1f;

        // Stunned (speedMult == 0): no damage ticks, no knockback
        if (speedMult <= 0f) return;

        playerInside.TakeDamage(damagePerSecond * speedMult * Time.deltaTime);

        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0f)
        {
            playerInside.TakeDamageWithKnockback(0f, transform.position);
            knockbackTimer = knockbackCooldown;
        }
    }
}
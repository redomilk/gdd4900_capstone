using System.Collections;
using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public float damagePerSecond = 10f;
    public float knockbackCooldown = 1f;

    PlayerStats playerInside;
    float knockbackTimer;

    void OnCollisionEnter2D(Collision2D col)
    {
        PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
        if (player != null)
        {
            playerInside = player;
            playerInside.TakeDamageWithKnockback(0f, transform.position);
            knockbackTimer = knockbackCooldown;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
        if (player != null)
            playerInside = player;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
        if (player != null && player == playerInside)
            playerInside = null;
    }

    void Update()
    {
        if (playerInside == null) return;

        EnemyHealth eh = GetComponent<EnemyHealth>();
        float speedMult = eh != null ? eh.GetSpeedMultiplier() : 1f;
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
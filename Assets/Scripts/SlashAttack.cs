using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    private Vector2 dir;
    private CoreEffects coreEffects;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    public void Initialize(Vector2 direction)
    {
        dir = direction;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (anim != null)
            anim.SetTrigger("Attack");

        Destroy(gameObject, 0.5f);
    }

    public void SetPlayerCollider(Collider2D playerCol)
    {
        Collider2D slashCol = GetComponent<Collider2D>();
        if (slashCol != null && playerCol != null)
            Physics2D.IgnoreCollision(slashCol, playerCol);
    }

    public void SetCoreEffects(CoreEffects effects)
    {
        coreEffects = effects;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Slash hit: " + other.gameObject.name + " | tag: " + other.tag);

        IDamageable damageable = other.GetComponent<IDamageable>()
                                ?? other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            float damage = 10f + (coreEffects != null ? coreEffects.totalDamageBonus : 0f);
            coreEffects?.SetLastMeleeDamage(damage);

            if (damageable is EnemyHealth enemyHealth)
                enemyHealth.TakeDamageWithKnockback(damage, transform.position, 8f);
            else
                damageable.TakeDamage(damage);

            coreEffects?.ApplyMeleeEffect(other.gameObject, dir);
        }

        if (other.TryGetComponent<Bullet>(out Bullet bullet))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = dir * rb.linearVelocity.magnitude;

            bullet.SetReflected(true);
        }
    }
}
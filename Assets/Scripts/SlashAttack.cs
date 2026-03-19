using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    public float lifetime = 0.12f;
    public float baseDamage = 10f;
    public float damage;
    public float knockback = 4f;
    public float reflectSpeedMultiplier = 1.5f;

    float timer;
    Vector2 slashDirection;
    Vector2 playerPosition;
    CoreEffects coreEffects;

    readonly HashSet<IDamageable> hitThisSwing = new HashSet<IDamageable>();

    Animator anim;

    void Awake()
    {
        damage = baseDamage;
        if (GameManager.instance != null)
            damage += GameManager.instance.damageLevel * GameManager.instance.damagePerLevel;

        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        timer = lifetime;
        hitThisSwing.Clear();

        // Play the animation when slash is activated
        if (anim != null)
            anim.Play("Slash");
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) Destroy(gameObject);
    }

    public void Initialize(Vector2 direction)
    {
        slashDirection = direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetPlayerPosition(Vector2 position) => playerPosition = position;

    public void SetCoreEffects(CoreEffects effects) => coreEffects = effects;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Reflect enemy bullets
        if (other.CompareTag("Enemy Bullet"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = slashDirection * rb.linearVelocity.magnitude * reflectSpeedMultiplier;
                Bullet enemyScript = other.GetComponent<Bullet>();
                if (enemyScript != null) Destroy(enemyScript);
                PlayerBullet pb = other.gameObject.AddComponent<PlayerBullet>();
                pb.baseDamage = damage;
                other.tag = "Player Bullet";
            }
            return;
        }

        // Hit crates
        CrateBreak crate = other.GetComponent<CrateBreak>();
        if (crate != null) { crate.TakeDamage(damage); return; }

        // Hit enemies
        if (!other.CompareTag("Enemy") && !other.CompareTag("Enemy Charger")) return;
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null) return;

        if (hitThisSwing.Add(dmg))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamageWithKnockback(damage, playerPosition, knockback);
                coreEffects?.SetLastMeleeDamage(damage);
                coreEffects?.ApplyMeleeEffect(enemy.gameObject, (enemy.transform.position - (Vector3)(Vector2)playerPosition).normalized);
            }
            else
            {
                dmg.TakeDamage(damage);
            }
        }
    }
}
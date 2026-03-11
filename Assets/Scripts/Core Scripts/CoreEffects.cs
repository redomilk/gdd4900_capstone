using System.Collections;
using UnityEngine;

// Attach to the Player. Reads active cores from CoreInventory and
// provides methods that weapon/projectile scripts call to apply effects.
[RequireComponent(typeof(CoreInventory))]
public class CoreEffects : MonoBehaviour
{
    CoreInventory inv;

    // Cached values - recalculated whenever a core is swapped
    [HideInInspector] public float totalDamageBonus;
    [HideInInspector] public float totalAttackSpeedBonus;
    [HideInInspector] public MeleeEffect activeMeleeEffect;
    [HideInInspector] public RangedEffect activeRangedEffect;

    void Awake() => inv = GetComponent<CoreInventory>();

    public void RecalculateFromInventory(CoreInventory inventory)
    {
        totalDamageBonus = inventory.TotalDamageBonus();
        totalAttackSpeedBonus = inventory.TotalAttackSpeedBonus();
        activeMeleeEffect = inventory.meleeCore != null ? inventory.meleeCore.meleeEffect : MeleeEffect.None;
        activeRangedEffect = inventory.rangedCore != null ? inventory.rangedCore.rangedEffect : RangedEffect.None;
    }

    // Call these from your melee/ranged weapon scripts

    // Call when a melee hit lands on target.
    public void ApplyMeleeEffect(GameObject target, Vector2 hitDirection)
    {
        CoreData core = inv.meleeCore;
        if (core == null) return;

        switch (core.meleeEffect)
        {
            case MeleeEffect.Knockback:
                ApplyKnockback(target, hitDirection, core.knockbackForce);
                break;

            case MeleeEffect.Bleed:
                StartCoroutine(ApplyBleed(target, core.bleedDPS, 4f));
                break;

            case MeleeEffect.Stun:
                ApplyStun(target, core.stunDuration);
                break;

            case MeleeEffect.Lifesteal:
                float rawDmg = GetLastMeleeDamage();
                float heal = rawDmg * core.lifestealPercent;
                PlayerStats ps = GetComponent<PlayerStats>();
                if (ps != null)
                {
                    ps.health = Mathf.Min(ps.health + heal, ps.maxHealth);
                    GameEvents.OnHealthChanged?.Invoke(ps.health, ps.maxHealth);
                }
                break;
        }
    }

    // Call from a projectile when it hits target.
    // Pass the projectile world position for AoE effects.
    public void ApplyRangedEffect(GameObject target, Vector2 hitPosition)
    {
        CoreData core = inv.rangedCore;
        if (core == null) return;

        switch (core.rangedEffect)
        {
            case RangedEffect.Freeze:
                ApplyFreeze(target, core.freezeDuration, core.freezeSlowPercent);
                break;

            case RangedEffect.Explosive:
                ApplyExplosion(hitPosition, core.explosionRadius, core.explosionDamage);
                break;

            case RangedEffect.Pierce:
                // Pierce is handled in the projectile itself.
                // Check: if (coreEffects.activeRangedEffect == RangedEffect.Pierce) skip destroy.
                break;

            case RangedEffect.Chain:
                StartCoroutine(ApplyChain(target, hitPosition, core.chainTargets));
                break;
        }
    }

    // Effect implementations

    void ApplyKnockback(GameObject target, Vector2 direction, float force)
    {
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    IEnumerator ApplyBleed(GameObject target, float dps, float duration)
    {
        float elapsed = 0f;
        float tick = 0.5f;

        while (elapsed < duration && target != null)
        {
            yield return new WaitForSeconds(tick);
            elapsed += tick;

            EnemyHealth eh = target.GetComponent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(dps * tick);
        }
    }

    void ApplyStun(GameObject target, float duration)
    {
        IStunnable stunnable = target.GetComponent<IStunnable>();
        if (stunnable != null)
            stunnable.Stun(duration);
        else
            StartCoroutine(RigidbodyStun(target, duration));
    }

    IEnumerator RigidbodyStun(GameObject target, float duration)
    {
        if (target == null) yield break;

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        RigidbodyType2D original = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Static;

        yield return new WaitForSeconds(duration);

        if (rb != null)
            rb.bodyType = original;
    }

    void ApplyFreeze(GameObject target, float duration, float slowPercent)
    {
        StartCoroutine(FreezeRoutine(target, duration, slowPercent));
    }

    IEnumerator FreezeRoutine(GameObject target, float duration, float slowPercent)
    {
        if (target == null) yield break;

        IStunnable stunnable = target.GetComponent<IStunnable>();
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (stunnable != null)
            stunnable.SetSpeedMultiplier(1f - slowPercent);
        else if (rb != null)
            rb.linearVelocity *= (1f - slowPercent);

        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        Color origColor = sr != null ? sr.color : Color.white;
        if (sr != null) sr.color = new Color(0.4f, 0.7f, 1f);

        yield return new WaitForSeconds(duration);

        if (target == null) yield break;

        if (stunnable != null) stunnable.SetSpeedMultiplier(1f);
        if (sr != null) sr.color = origColor;
    }

    void ApplyExplosion(Vector2 centre, float radius, float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(centre, radius);
        foreach (var col in hits)
        {
            if (col.gameObject == gameObject) continue;

            EnemyHealth eh = col.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                float dist = Vector2.Distance(centre, col.transform.position);
                float falloff = Mathf.Clamp01(1f - dist / radius);
                eh.TakeDamage(damage * falloff);
            }

            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = ((Vector2)col.transform.position - centre).normalized;
                rb.AddForce(dir * damage * 0.5f, ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator ApplyChain(GameObject firstTarget, Vector2 origin, int remaining)
    {
        if (remaining <= 0 || firstTarget == null) yield break;

        yield return new WaitForEndOfFrame();

        Collider2D[] nearby = Physics2D.OverlapCircleAll(firstTarget.transform.position, 6f);
        GameObject next = null;
        float closest = float.MaxValue;

        foreach (var col in nearby)
        {
            if (col.gameObject == firstTarget || col.gameObject == gameObject) continue;
            if (col.GetComponent<EnemyHealth>() == null) continue;

            float d = Vector2.Distance(firstTarget.transform.position, col.transform.position);
            if (d < closest) { closest = d; next = col.gameObject; }
        }

        if (next != null)
        {
            EnemyHealth eh = next.GetComponent<EnemyHealth>();
            CoreData core = inv.rangedCore;
            if (eh != null && core != null)
                eh.TakeDamage(core.damageBonus * 0.6f);

            StartCoroutine(ApplyChain(next, firstTarget.transform.position, remaining - 1));
        }
    }

    // Utility

    float _lastMeleeDamage = 10f;
    public void SetLastMeleeDamage(float dmg) => _lastMeleeDamage = dmg;
    float GetLastMeleeDamage() => _lastMeleeDamage;
}
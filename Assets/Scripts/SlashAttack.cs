using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    public float lifetime = 0.12f;
    public float damage = 10f;

    float timer;

    // Prevent hitting the same target multiple times during one slash
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
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // only hit objs with enemy tag
        if (!other.CompareTag("Enemy")) return;

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null) return;

        if (hitThisSwing.Add(dmg))
            dmg.TakeDamage(damage);
    }
}
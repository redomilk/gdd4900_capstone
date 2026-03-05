using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float baseDamage = 10f;
    public float damage;
    public float knockback = 4f;
    public float lifetime = 4f;

    void Awake()
    {
        // Apply damage upgrade on top of base damage
        damage = baseDamage;
        if (GameManager.instance != null)
            damage += GameManager.instance.damageLevel * GameManager.instance.damagePerLevel;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamageWithKnockback(damage, transform.position, knockback);  // CHANGED
            Destroy(gameObject);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            Destroy(gameObject);
    }
}
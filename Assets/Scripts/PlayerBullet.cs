using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            Destroy(gameObject);
    }
}
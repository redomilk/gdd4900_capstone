using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 4f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.TakeDamageWithKnockback(damage, transform.position); 
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            Destroy(gameObject);
    }
}
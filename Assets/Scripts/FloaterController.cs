using UnityEngine;
public class FloaterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float raycastDistance = 0.6f;
    [Header("Damage")]
    public float damagePerSecond = 10f;
    Rigidbody2D rb;
    int wallLayer;
    float direction = 1f;
    float knockbackTimer;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        wallLayer = LayerMask.GetMask("Wall");
    }
    public void ApplyKnockback(float duration)
    {
        knockbackTimer = duration;
    }
    void FixedUpdate()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, raycastDistance, wallLayer);
        if (hit.collider != null)
            direction *= -1f;
        float mult = GetComponent<EnemyHealth>()?.GetSpeedMultiplier() ?? 1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed * mult, rb.linearVelocity.y);
    }
    void OnCollisionStay2D(Collision2D col)
    {
        PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
        if (player != null)
            player.TakeDamage(damagePerSecond * Time.deltaTime);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * direction * raycastDistance);
    }
}
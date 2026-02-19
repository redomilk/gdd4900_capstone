using UnityEngine;

public class FloaterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float raycastDistance = 0.6f; // how close to wall before turning

    [Header("Damage")]
    public float damagePerSecond = 10f;

    Rigidbody2D rb;
    int wallLayer;
    float direction = 1f; // 1 = right, -1 = left

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        wallLayer = LayerMask.GetMask("Wall");
    }

    void FixedUpdate()
    {
        // Raycast ahead to detect wall
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, raycastDistance, wallLayer);

        if (hit.collider != null)
            direction *= -1f; // flip direction

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
        if (player != null)
            player.TakeDamage(damagePerSecond * Time.deltaTime);
    }

    // Visualize raycast in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * direction * raycastDistance);
    }
}
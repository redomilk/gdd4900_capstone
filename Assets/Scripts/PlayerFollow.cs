using UnityEngine;
public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float stopDistance = 0.5f;
    public float followDistance = 5f;
    float knockbackTimer;

    //charager knockback
    public float chargerKnockbackForce = 8f;
    public float chargerKnockbackDuration = 0.3f;
    public void ApplyKnockback(float duration)
    {
        knockbackTimer = duration;
    }
    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            return;
        }
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
            return;
        }
        Vector2 currentPos = transform.position;
        Vector2 targetPos = player.position;
        float distance = Vector2.Distance(currentPos, targetPos);
        if (distance < followDistance)
        {
            Vector2 direction = (Vector2)player.position - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            if (distance > stopDistance)
            {
                float mult = GetComponent<EnemyHealth>()?.GetSpeedMultiplier() ?? 1f;
                Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, speed * mult * Time.deltaTime);
                transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            }
        }
    }

    //knockback charger on player collision
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        if (!CompareTag("Enemy Charger")) return;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;
        Vector2 dir = ((Vector2)transform.position - (Vector2)col.transform.position).normalized;
        rb.AddForce(dir * chargerKnockbackForce, ForceMode2D.Impulse);
        ApplyKnockback(chargerKnockbackDuration);
    }
}
using UnityEngine;
using System.Collections;

public class TurretEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 8f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float bulletSpeed = 6f;

    [Header("Burst Mode")]
    public bool burstMode = false;
    public int burstCount = 3;
    public float burstDelay = 0.12f;

    Transform player;
    float fireTimer;
    bool isBursting;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRadius) return;

        // Rotate toward player
        Vector2 direction = (player.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f && !isBursting)
        {
            if (burstMode)
            {
                StartCoroutine(BurstShoot(direction));
            }
            else
            {
                Shoot(direction);
            }

            fireTimer = fireRate;
        }
    }

    IEnumerator BurstShoot(Vector2 direction)
    {
        isBursting = true;

        for (int i = 0; i < burstCount; i++)
        {
            Shoot(direction);
            yield return new WaitForSeconds(burstDelay);
        }

        isBursting = false;
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
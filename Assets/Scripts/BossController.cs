using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    public float detectionRange = 10f;
    public float moveSpeed = 1.2f;

    [Header("Spawning")]
    public GameObject[] enemyPrefabs;       
    public Transform[] spawnPoints;         // child objects marking where enemies spawn
    public float spawnInterval = 4f;        // seconds between spawns
    public int maxSpawnedEnemies = 6;       // won't spawn more if this many are alive
    public int enemiesPerSpawn = 1;         // how many spawn each interval

    [Header("Damage")]
    public float contactDamage = 15f;
    public float contactCooldown = 1f;

    Transform player;
    Rigidbody2D rb;
    float spawnTimer;
    float contactTimer;
    int currentSpawnedCount = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        EnemyHealth eh = GetComponent<EnemyHealth>();
        if (eh != null) eh.maxHealth = 300f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj == null)
            Debug.LogWarning("Boss could not find Player");
        else
            player = playerObj.transform;
    }

    void Update()
    {
        if (player != null)
        {
            HandleMovement();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        HandleSpawning();

        if (contactTimer > 0f)
            contactTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleSpawning()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && currentSpawnedCount < maxSpawnedEnemies)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to boss.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned to boss.");
            return;
        }

        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            if (currentSpawnedCount >= maxSpawnedEnemies) break;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            GameObject spawned = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            currentSpawnedCount++;

            BossSpawnedEnemy tracker = spawned.AddComponent<BossSpawnedEnemy>();
            tracker.boss = this;

            Debug.Log("Boss spawned enemy: " + prefab.name);
        }
    }

    public void OnSpawnedEnemyDied()
    {
        currentSpawnedCount = Mathf.Max(0, currentSpawnedCount - 1);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (contactTimer > 0f) return;
        if (!col.gameObject.CompareTag("Player")) return;

        PlayerStats stats = col.gameObject.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamageWithKnockback(contactDamage, transform.position);
            contactTimer = contactCooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
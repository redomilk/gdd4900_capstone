using UnityEngine;
public class EnemyExploder : MonoBehaviour
{
    [Header("Explosion")]
    public float explodeRadius = 2f;
    public float explodeDamage = 25f;
    public float triggerDistance = 1.2f;

    [Header("Windup")]
    public float fuseTime = 0.8f;
    public float flashInterval = 0.1f;

    [Header("Effects")]
    public GameObject explosionVFXPrefab;
    public GameObject explosionLightPrefab;

    Transform player;
    SpriteRenderer sr;
    bool isTriggered = false;
    float fuseTimer;
    float flashTimer;
    bool flashState;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
            return;
        }

        if (isTriggered)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                flashState = !flashState;
                sr.color = flashState ? Color.red : Color.white;
                flashTimer = flashInterval;
            }

            fuseTimer -= Time.deltaTime;
            if (fuseTimer <= 0f)
                Explode();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= triggerDistance)
        {
            isTriggered = true;
            fuseTimer = fuseTime;
            flashTimer = flashInterval;
        }
    }

    void Explode()
    {
        if (player != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            if (distToPlayer <= explodeRadius)
            {
                PlayerStats ps = player.GetComponent<PlayerStats>();
                if (ps != null) ps.TakeDamageWithKnockback(explodeDamage, transform.position); 
            }
        }

        // spawn VFX
        GameObject vfx = null;
        if (explosionVFXPrefab != null)
            vfx = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);

        // spawn light and parent it to the VFX
        if (explosionLightPrefab != null)
        {
            var lightObj = Instantiate(explosionLightPrefab, transform.position, Quaternion.identity);
            if (vfx != null) lightObj.transform.SetParent(vfx.transform, true);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
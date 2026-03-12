using UnityEngine;

public class CrateBreak : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 20f;
    float health;

    [Header("Core Drop")]
    public GameObject spawnTableObject;
    public GameObject corePickupPrefab;

    [Header("Effects")]
    public GameObject breakVFXPrefab;
    [Tooltip("FMOD event path, e.g. event:/SFX_CrateBreak")]
    public string breakSoundEvent = "event:/SFX_CrateBreak";

    SpriteRenderer sr;

    void Awake()
    {
        health = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        health = Mathf.Max(0f, health - amount);

        if (sr != null)
            StartCoroutine(HitFlash());

        if (health <= 0f)
            Break();
    }

    public void TakeDamageWithKnockback(float amount, Vector2 sourcePosition, float force)
    {
        TakeDamage(amount);
    }

    void Break()
    {
        if (breakVFXPrefab != null)
            Instantiate(breakVFXPrefab, transform.position, Quaternion.identity);

        // Play one-shot FMOD event at this position
        if (!string.IsNullOrEmpty(breakSoundEvent))
            FMODUnity.RuntimeManager.PlayOneShot(breakSoundEvent, transform.position);

        SpawnCoreDrop();
        Destroy(gameObject);
    }

    void SpawnCoreDrop()
    {
        if (corePickupPrefab == null)
        {
            Debug.LogWarning("CrateBreak: corePickupPrefab is not assigned.");
            return;
        }

        // Find CoreSpawnTable anywhere in the scene, including DontDestroyOnLoad objects
        CoreSpawnTable spawnTable = null;

        if (spawnTableObject != null)
            spawnTable = spawnTableObject.GetComponent<CoreSpawnTable>();

        // Fallback: search all objects including persistent ones
        if (spawnTable == null)
            spawnTable = FindFirstObjectByType<CoreSpawnTable>();

        if (spawnTable == null)
        {
            Debug.LogWarning("CrateBreak: Could not find CoreSpawnTable anywhere in scene.");
            return;
        }

        CoreData rolled = spawnTable.RollCore(transform.position.y);

        GameObject go = Instantiate(corePickupPrefab, transform.position, Quaternion.identity);
        CoreSwapPickup pickup = go.GetComponent<CoreSwapPickup>();

        if (pickup != null)
            pickup.Initialize(rolled);
        else
            Debug.LogWarning("CrateBreak: corePickupPrefab does not have a CoreSwapPickup component.");
    }

    System.Collections.IEnumerator HitFlash()
    {
        if (sr == null) yield break;
        Color orig = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.06f);
        if (sr != null) sr.color = orig;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.4f);
        Gizmos.DrawCube(transform.position, Vector3.one * 0.8f);
    }
}
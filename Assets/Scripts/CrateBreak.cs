using UnityEngine;

public class CrateBreak : MonoBehaviour, IDamageable
{
    private static bool shownCoreTipThisSession = false;

    [Header("Health")]
    public float maxHealth = 20f;
    private float health;

    [Header("Core Drop")]
    public GameObject spawnTableObject;
    public GameObject corePickupPrefab;

    [Header("Effects")]
    public GameObject breakVFXPrefab;
    [Tooltip("FMOD event path, e.g. event:/SFX_CrateBreak")]
    public string breakSoundEvent = "event:/SFX_CrateBreak";

    private SpriteRenderer sr;

    void Awake()
    {
        health = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetSessionFlag()
    {
        shownCoreTipThisSession = false;
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

        if (!string.IsNullOrEmpty(breakSoundEvent))
            FMODUnity.RuntimeManager.PlayOneShot(breakSoundEvent, transform.position);

        if (!shownCoreTipThisSession && TooltipPopup.Instance != null)
        {
            shownCoreTipThisSession = true;

            TooltipPopup.Instance.ShowOnce(
                "tip_augments",
                "Augment Crates",
                "Broken crates can contain augments.\n\n" +
                "Augments grant powerful passive effects.\n\n" +
                "Press <color=yellow>E</color> near an augment to equip\n\n" +
                "Press <color=yellow>TAB</color> to view current cores."
            );
        }

        SpawnCoreDrop();
        Destroy(gameObject);
    }

    void SpawnCoreDrop()
    {
        Debug.Log("Crate broke. Trying to spawn core.");

        if (corePickupPrefab == null)
        {
            Debug.LogWarning("CrateBreak: corePickupPrefab is not assigned.");
            return;
        }

        CoreSpawnTable spawnTable = null;

        if (spawnTableObject != null)
            spawnTable = spawnTableObject.GetComponent<CoreSpawnTable>();

        if (spawnTable == null)
            spawnTable = FindFirstObjectByType<CoreSpawnTable>(FindObjectsInactive.Include);

        Debug.Log("CoreSpawnTable found: " + (spawnTable != null));

        if (spawnTable == null)
        {
            Debug.LogWarning("CrateBreak: Could not find CoreSpawnTable anywhere in scene.");
            return;
        }

        CoreData rolled = spawnTable.RollCore(transform.position.y);

        if (rolled == null)
        {
            Debug.LogWarning("CrateBreak: RollCore returned null.");
            return;
        }

        GameObject go = Instantiate(corePickupPrefab, transform.position, Quaternion.identity);
        CoreSwapPickup pickup = go.GetComponent<CoreSwapPickup>();

        if (pickup == null)
        {
            Debug.LogWarning("CrateBreak: corePickupPrefab does not have a CoreSwapPickup component.");
            return;
        }

        pickup.Initialize(rolled);
    }

    System.Collections.IEnumerator HitFlash()
    {
        if (sr == null) yield break;

        Color originalColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(0.06f);

        if (sr != null)
            sr.color = originalColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.4f);
        Gizmos.DrawCube(transform.position, Vector3.one * 0.8f);
    }
}
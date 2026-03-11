using UnityEngine;
using UnityEngine.InputSystem;

/// Attach to a world-space core pickup object alongside a SpriteRenderer and Collider2D (trigger).
[RequireComponent(typeof(SpriteRenderer))]
public class CoreSwapPickup : MonoBehaviour
{
    public CoreData coreData;

    bool playerNearby;
    bool collected;
    bool collectQueued;
    SpriteRenderer sr;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    void Start()
    {
        if (coreData == null) return;

        if (sr != null)
        {
            sr.sprite = coreData.icon;
            // Tint the glow ring / outline to rarity colour if you have a child object for it
            ApplyRarityVisuals();
        }
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            collectQueued = true;

        if (collected) return;

        if (playerNearby && collectQueued)
        {
            collectQueued = false;
            Collect();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;

        if (coreData != null)
        {
            string rarityTag = $"<color=#{ColorUtility.ToHtmlStringRGB(coreData.RarityColor())}>[{coreData.RarityLabel()}]</color>";
            PlayerHUD.instance?.ShowPrompt($"E  to equip {rarityTag} {coreData.coreName}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        PlayerHUD.instance?.HidePrompt();
    }

    void Collect()
    {
        collected = true;
        PlayerHUD.instance?.HidePrompt();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("CorePickup: Player not found.");
            return;
        }

        CoreInventory inventory = player.GetComponent<CoreInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("CorePickup: No CoreInventory on player.");
            return;
        }

        CoreData dropped = inventory.Equip(coreData);

        // Spawn the displaced core as a new pickup at this position
        if (dropped != null && inventory.corePickupPrefab != null)
        {
            GameObject go = Instantiate(inventory.corePickupPrefab, transform.position, Quaternion.identity);
            CoreSwapPickup cp = go.GetComponent<CoreSwapPickup>();
            if (cp != null) cp.coreData = dropped;
        }

        Destroy(gameObject);
    }

    void ApplyRarityVisuals()
    {
        if (coreData == null) return;

        // Optional: if you have a child GameObject called "RarityGlow" with its own SpriteRenderer
        Transform glow = transform.Find("RarityGlow");
        if (glow != null)
        {
            SpriteRenderer glowSr = glow.GetComponent<SpriteRenderer>();
            if (glowSr != null)
                glowSr.color = coreData.RarityColor();
        }
    }
}
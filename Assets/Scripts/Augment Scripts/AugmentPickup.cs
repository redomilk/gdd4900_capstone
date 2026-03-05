using UnityEngine;
using UnityEngine.InputSystem;

public class AugmentPickup : MonoBehaviour
{
    public AugmentData augmentData;

    bool playerNearby;
    bool collected;
    bool collectQueued;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (augmentData != null && sr != null)
            sr.sprite = augmentData.icon;
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
        PlayerHUD.instance?.ShowPrompt("E to equip " + augmentData.augmentName);
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
            Debug.LogWarning("Player not found in AugmentPickup.Collect");
            return;
        }

        AugmentInventory inventory = player.GetComponent<AugmentInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("No AugmentInventory on player");
            return;
        }

        AugmentData dropped = inventory.Equip(augmentData);

        // Spawn dropped augment as a new pickup
        if (dropped != null && inventory.augmentPickupPrefab != null)
        {
            GameObject go = Instantiate(inventory.augmentPickupPrefab, transform.position, Quaternion.identity);
            go.GetComponent<AugmentPickup>().augmentData = dropped;
        }

        Destroy(gameObject);
    }
}
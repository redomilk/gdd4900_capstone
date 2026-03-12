using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class CoreSwapPickup : MonoBehaviour
{
    public CoreData coreData;

    bool playerNearby;
    bool collected;
    bool collectQueued;
    SpriteRenderer sr;
    SpriteRenderer glowSr;

    // Safe getter - works even if called before Awake
    SpriteRenderer SR => sr != null ? sr : (sr = GetComponent<SpriteRenderer>());

    void Awake() => sr = GetComponent<SpriteRenderer>();

    void Start()
    {
        if (coreData != null)
            ApplyCoreData();
    }

    public void Initialize(CoreData data)
    {
        coreData = data;
        Debug.Log("CoreSwapPickup.Initialize called, data = " + (data != null ? data.coreName : "NULL"));
        ApplyCoreData();
    }

    void ApplyCoreData()
    {
        if (coreData == null) { Debug.LogWarning("CoreSwapPickup.ApplyCoreData: coreData is null"); return; }
       // Debug.Log("CoreSwapPickup.ApplyCoreData: applying " + coreData.coreName + " icon=" + (coreData.icon != null ? coreData.icon.name : "NULL"));
        SR.sprite = coreData.icon;
        SetupGlow();
    }

    void SetupGlow()
    {
        if (coreData == null) return;

        Transform glowTransform = transform.Find("RarityGlow");

        if (glowTransform == null)
        {
            GameObject glowObj = new GameObject("RarityGlow");
            glowObj.transform.SetParent(transform, false);
            glowObj.transform.localPosition = Vector3.zero;
            glowTransform = glowObj.transform;
        }

        glowSr = glowTransform.GetComponent<SpriteRenderer>();
        if (glowSr == null)
            glowSr = glowTransform.gameObject.AddComponent<SpriteRenderer>();

        glowSr.sprite = CreateGlowSprite(128);
        glowSr.material = CreateAdditiveMaterial();
        glowSr.sortingOrder = SR.sortingOrder + 1;

        glowSr.color = WithAlpha(coreData.RarityColor(), RarityAlpha(coreData.rarity));

        float scale = RarityScale(coreData.rarity);
        glowTransform.localScale = new Vector3(scale, scale, 1f);
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

        if (glowSr != null && coreData != null)
        {
            float baseAlpha = RarityAlpha(coreData.rarity);
            float pulse = baseAlpha + Mathf.Sin(Time.time * 2f) * 0.08f;
            glowSr.color = WithAlpha(coreData.RarityColor(), Mathf.Clamp01(pulse));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;

        if (coreData != null)
        {
            string rarityTag = $"<color=#{ColorUtility.ToHtmlStringRGB(coreData.RarityColor())}>[{coreData.RarityLabel()}]</color>";
            PlayerHUD.instance?.ShowPrompt($"E to equip {rarityTag} {coreData.coreName}");
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
        if (player == null) { Debug.LogWarning("CoreSwapPickup: Player not found."); return; }

        CoreInventory inventory = player.GetComponent<CoreInventory>();
        if (inventory == null) { Debug.LogWarning("CoreSwapPickup: No CoreInventory on player."); return; }

        CoreData dropped = inventory.Equip(coreData);

        if (dropped != null && inventory.corePickupPrefab != null)
        {
            GameObject go = Instantiate(inventory.corePickupPrefab, transform.position, Quaternion.identity);
            CoreSwapPickup pickup = go.GetComponent<CoreSwapPickup>();
            if (pickup != null) pickup.Initialize(dropped);
        }

        Destroy(gameObject);
    }

    static Sprite CreateGlowSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        float center = size * 0.5f;
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float t = Mathf.Clamp01(dist / radius);

                // Sharp bright core in the centre
                float core = Mathf.Pow(Mathf.Clamp01(1f - t * 4f), 2f);
                // Soft outer glow that falls off quickly
                float outer = Mathf.Pow(1f - t, 4f) * 0.5f;

                float alpha = Mathf.Clamp01(core + outer);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    static Material CreateAdditiveMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        return mat;
    }

    Color WithAlpha(Color c, float a) { c.a = a; return c; }

    float RarityAlpha(CoreRarity rarity) => rarity switch
    {
        CoreRarity.Common => 0.6f,
        CoreRarity.Uncommon => 0.7f,
        CoreRarity.Rare => 0.8f,
        CoreRarity.Epic => 0.9f,
        CoreRarity.UltraRare => 1.0f,
        _ => 0.6f
    };

    float RarityScale(CoreRarity rarity) => rarity switch
    {
        CoreRarity.Common => 0.8f,
        CoreRarity.Uncommon => 1.0f,
        CoreRarity.Rare => 1.2f,
        CoreRarity.Epic => 1.5f,
        CoreRarity.UltraRare => 1.9f,
        _ => 0.8f
    };
}
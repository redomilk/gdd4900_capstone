using UnityEngine;

// saves equipped cores on extraction and restores them on scene load.
public class CorePersistence : MonoBehaviour
{
    public static CorePersistence instance;

    // Stored core names - empty string means no core in that slot
    [HideInInspector] public string savedMainCore = "";
    [HideInInspector] public string savedMeleeCore = "";
    [HideInInspector] public string savedRangedCore = "";
    [HideInInspector] public string savedBoosterCore = "";

    // All CoreData assets 
    [Header("Core Registry")]
    public CoreData[] allCores;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this from extraction zone script when the player successfully extracts
    public void SaveCores()
    {
        CoreInventory inv = GetPlayerInventory();
        if (inv == null) return;

        savedMainCore = inv.mainCore != null ? inv.mainCore.name : "";
        savedMeleeCore = inv.meleeCore != null ? inv.meleeCore.name : "";
        savedRangedCore = inv.rangedCore != null ? inv.rangedCore.name : "";
        savedBoosterCore = inv.boosterCore != null ? inv.boosterCore.name : "";

        Debug.Log("SaveCores: main=" + savedMainCore + " melee=" + savedMeleeCore + " ranged=" + savedRangedCore + " booster=" + savedBoosterCore);
    }

    // Call this from death/wipe script when the player dies
    public void WipeCores()
    {
        savedMainCore = "";
        savedMeleeCore = "";
        savedRangedCore = "";
        savedBoosterCore = "";
        Debug.Log("Cores wiped on death.");
    }

    // Call this after scene load to re-equip saved cores
    public void RestoreCores()
    {
        Debug.Log("RestoreCores: main=" + savedMainCore + " melee=" + savedMeleeCore + " ranged=" + savedRangedCore + " booster=" + savedBoosterCore);
        Debug.Log("allCores length: " + (allCores != null ? allCores.Length.ToString() : "NULL"));
        if (allCores == null || allCores.Length == 0)
        {
            Debug.LogWarning("CorePersistence: allCores array is empty.");
            return;
        }

        CoreInventory inv = GetPlayerInventory();
        if (inv == null) return;

        TryEquip(inv, savedMainCore);
        TryEquip(inv, savedMeleeCore);
        TryEquip(inv, savedRangedCore);
        TryEquip(inv, savedBoosterCore);

        Debug.Log("Cores restored after scene load.");
    }

    void TryEquip(CoreInventory inv, string coreName)
    {
        if (string.IsNullOrEmpty(coreName)) return;

        foreach (CoreData core in allCores)
        {
            if (core.name == coreName)
            {
                inv.Equip(core);
                return;
            }
        }

        Debug.LogWarning("CorePersistence: Could not find core named " + coreName);
    }

    CoreInventory GetPlayerInventory()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { Debug.LogWarning("CorePersistence: Player not found."); return null; }
        CoreInventory inv = player.GetComponent<CoreInventory>();
        if (inv == null) Debug.LogWarning("CorePersistence: No CoreInventory on player.");
        return inv;
    }
}
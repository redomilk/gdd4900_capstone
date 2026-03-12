using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Place on a manager object. Handles depth-weighted random core spawning.
// Assign all CoreData assets to the corePool list in the Inspector.
public class CoreSpawnTable : MonoBehaviour
{
    [Header("Core Pool")]
    [Tooltip("Drag ALL CoreData ScriptableObjects here.")]
    public List<CoreData> corePool = new();

    [Header("Spawn Settings")]
    public GameObject corePickupPrefab;

    [Header("Debug Spawning")]
    [Tooltip("When enabled, all crate drops will force this rarity instead of rolling randomly.")]
    public bool forceRarity = false;
    public CoreRarity forcedRarity = CoreRarity.UltraRare;

    // Returns a random CoreData valid for the given depth (world-space Y, negative = deeper).
    public CoreData RollCore(float currentDepth)
    {
        Debug.Log("RollCore called, pool count: " + corePool.Count + " depth: " + currentDepth);

        if (corePool == null || corePool.Count == 0)
        {
            Debug.LogWarning("CoreSpawnTable: corePool is empty!");
            return null;
        }

        // Debug override - force a specific rarity
        if (forceRarity)
        {
            List<CoreData> ofRarity = corePool.Where(c => c.rarity == forcedRarity).ToList();
            if (ofRarity.Count > 0)
            {
                CoreData forced = ofRarity[Random.Range(0, ofRarity.Count)];
                Debug.Log("CoreSpawnTable: Force-spawning " + forced.coreName + " [" + forcedRarity + "]");
                return forced;
            }
            Debug.LogWarning("CoreSpawnTable: No cores of rarity " + forcedRarity + " in pool.");
        }

        float depth = Mathf.Abs(currentDepth);

        List<CoreData> eligible = corePool
            .Where(c => depth >= CoreData.MinDepthForRarity(c.rarity))
            .ToList();

        if (eligible.Count == 0)
        {
            Debug.LogWarning("CoreSpawnTable: No eligible cores at depth " + depth);
            return null;
        }

        float totalWeight = eligible.Sum(c => GetWeight(c.rarity, depth));
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var core in eligible)
        {
            cumulative += GetWeight(core.rarity, depth);
            if (roll <= cumulative)
                return core;
        }

        return eligible[eligible.Count - 1];
    }

    public GameObject SpawnCorePickup(Vector3 position, float currentDepth)
    {
        CoreData rolled = RollCore(currentDepth);
        if (rolled == null || corePickupPrefab == null) return null;

        GameObject go = Instantiate(corePickupPrefab, position, Quaternion.identity);
        CoreSwapPickup cp = go.GetComponent<CoreSwapPickup>();
        if (cp != null) cp.Initialize(rolled);
        return go;
    }

    // Weight table
    static float GetWeight(CoreRarity rarity, float depth)
    {
        return rarity switch
        {
            CoreRarity.Common => Mathf.Max(5f, 100f - depth * 0.15f),
            CoreRarity.Uncommon => 60f + Mathf.Max(0f, (depth - 50f) * 0.10f),
            CoreRarity.Rare => 25f + Mathf.Max(0f, (depth - 120f) * 0.12f),
            CoreRarity.Epic => 8f + Mathf.Max(0f, (depth - 220f) * 0.08f),
            CoreRarity.UltraRare => 2f + Mathf.Max(0f, (depth - 350f) * 0.05f),
            _ => 0f
        };
    }

#if UNITY_EDITOR
    [ContextMenu("Debug: Print Odds at Depth 0")]
    void DebugOddsShallow() => PrintOdds(0f);
    [ContextMenu("Debug: Print Odds at Depth 200")]
    void DebugOddsMid() => PrintOdds(200f);
    [ContextMenu("Debug: Print Odds at Depth 400")]
    void DebugOddsDeep() => PrintOdds(400f);

    void PrintOdds(float depth)
    {
        float total = System.Enum.GetValues(typeof(CoreRarity))
                          .Cast<CoreRarity>()
                          .Sum(r => GetWeight(r, depth));

        foreach (CoreRarity r in System.Enum.GetValues(typeof(CoreRarity)))
        {
            float w = GetWeight(r, depth);
            Debug.Log("Depth " + depth + " | " + r + " weight=" + w.ToString("0.0") + " (" + (w / total * 100f).ToString("0.0") + "%)");
        }
    }
#endif
}
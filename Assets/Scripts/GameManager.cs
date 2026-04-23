using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int scrapCount = 0;
    public int runScrapCount = 0;
    public int healthLevel = 0;
    public int speedLevel = 0;
    public int oxygenLevel = 0;
    public int damageLevel = 0;

    [Header("Stat Per Level")]
    public float hpPerLevel = 20f;
    public float oxygenPerLevel = 10f;
    public float speedPerLevel = 0.3f;
    public float damagePerLevel = 2f;

    [Header("Run Summary")]
    public int lastRunScrapCollected;
    public int lastRunScrapLost;
    public int lastRunScrapExtracted;
    public float lastRunTime;
    public bool lastRunExtracted;

    private float runStartTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu") return;

        PlayerStats ps = FindFirstObjectByType<PlayerStats>();
        if (ps != null) ApplyUpgrades(ps);

        PlayerDiveController dc = FindFirstObjectByType<PlayerDiveController>();
        if (dc != null) ApplySpeedUpgrade(dc);

        PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null)
        {
            Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Canvas canvas in allCanvases)
            {
                Transform[] allChildren = canvas.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in allChildren)
                {
                    if (t.name == "PausePanel") pauseMenu.pausePanel = t.gameObject;
                    if (t.name == "OptionsPanel") pauseMenu.optionsPanel = t.gameObject;
                }
            }

            if (pauseMenu.pausePanel != null)
            {
                CanvasGroup cg = pauseMenu.pausePanel.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.interactable = true;
                    cg.alpha = 1f;
                }
                pauseMenu.RewireButtons();
            }
        }

        CorePersistence.instance?.RestoreCores();
    }

    public void StartRun()
    {
        runScrapCount = 0;

        lastRunScrapCollected = 0;
        lastRunScrapLost = 0;
        lastRunScrapExtracted = 0;
        lastRunTime = 0f;
        lastRunExtracted = false;

        runStartTime = Time.time;
    }

    public void ApplyUpgrades(PlayerStats ps)
    {
        ps.maxHealth = 100f + (healthLevel * hpPerLevel);
        ps.health = ps.maxHealth;

        ps.maxOxygen = 100f + (oxygenLevel * oxygenPerLevel);
        ps.oxygen = ps.maxOxygen;

        GameEvents.OnHealthChanged?.Invoke(ps.health, ps.maxHealth);
        GameEvents.OnOxygenChanged?.Invoke(ps.oxygen, ps.maxOxygen);
    }

    public void ApplySpeedUpgrade(PlayerDiveController dc)
    {
        dc.maxHorizontalSpeed = 4.5f + (speedLevel * speedPerLevel);
        dc.maxVerticalSpeed = 4.5f + (speedLevel * speedPerLevel);
    }

    public void AddScrap(int amount)
    {
        runScrapCount += amount;
        lastRunScrapCollected += amount;
    }

    public void BankRunScrap()
    {
        scrapCount += runScrapCount;
        runScrapCount = 0;
    }

    public void DeathScrapPenalty()
    {
        int lost = Mathf.FloorToInt(runScrapCount * 0.75f);
        int kept = runScrapCount - lost;
        scrapCount += kept;
        runScrapCount = 0;
        Debug.Log($"Lost {lost} run scrap on death, banked {kept}");
        CorePersistence.instance?.WipeCores();
    }

    public void CompleteRunExtract()
    {
        lastRunExtracted = true;
        lastRunTime = Time.time - runStartTime;
        lastRunScrapLost = 0;
        lastRunScrapExtracted = runScrapCount;

        BankRunScrap();
        CorePersistence.instance?.SaveCores();
    }

    public void CompleteRunDeath()
    {
        lastRunExtracted = false;
        lastRunTime = Time.time - runStartTime;

        int lost = Mathf.FloorToInt(runScrapCount * 0.75f);
        int kept = runScrapCount - lost;

        lastRunScrapLost = lost;
        lastRunScrapExtracted = kept;

        DeathScrapPenalty();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            healthLevel++;
            PlayerStats ps = FindFirstObjectByType<PlayerStats>();
            if (ps != null) ApplyUpgrades(ps);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            speedLevel++;
            PlayerDiveController dc = FindFirstObjectByType<PlayerDiveController>();
            if (dc != null) ApplySpeedUpgrade(dc);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            oxygenLevel++;
            PlayerStats ps = FindFirstObjectByType<PlayerStats>();
            if (ps != null) ApplyUpgrades(ps);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            damageLevel++;

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            PlayerStats ps = FindFirstObjectByType<PlayerStats>();
            PlayerDiveController dc = FindFirstObjectByType<PlayerDiveController>();
            Debug.Log("=== PLAYER STATS ===");
            Debug.Log($"Health: {ps?.health} / {ps?.maxHealth} (level {healthLevel})");
            Debug.Log($"Oxygen: {ps?.oxygen} / {ps?.maxOxygen} (level {oxygenLevel})");
            Debug.Log($"Speed H: {dc?.maxHorizontalSpeed} V: {dc?.maxVerticalSpeed} (level {speedLevel})");
            Debug.Log($"Damage level: {damageLevel} (+{damageLevel * damagePerLevel} damage)");
            Debug.Log($"Knockback Force: {ps?.knockbackForce} Duration: {ps?.knockbackDuration}");
            Debug.Log($"Scrap stash: {scrapCount} | Run scrap: {runScrapCount}");
            Debug.Log("====================");
        }
    }
}
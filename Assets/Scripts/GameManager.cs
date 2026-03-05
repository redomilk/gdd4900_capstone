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
        PlayerStats ps = FindFirstObjectByType<PlayerStats>();
        if (ps != null) ApplyUpgrades(ps);

        PlayerDiveController dc = FindFirstObjectByType<PlayerDiveController>();
        if (dc != null) ApplySpeedUpgrade(dc);

        PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null)
        {
            // Search entire scene for panels by name including inactive
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

            Debug.Log($"PauseMenu found. pausePanel: {pauseMenu.pausePanel}, optionsPanel: {pauseMenu.optionsPanel}");

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

        MainMenu mainMenu = FindFirstObjectByType<MainMenu>();
        if (mainMenu != null)
        {
            mainMenu.menuCanvas = GameObject.Find("Main Menu Canvas");
            mainMenu.player = GameObject.FindGameObjectWithTag("Player");
            mainMenu.depthGauge = GameObject.Find("Depth Guage UI");
            mainMenu.resourceBars = GameObject.Find("Resource Bars");
        }
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
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Press 1 to add health level
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            healthLevel++;
            Debug.Log($"Health level: {healthLevel}");
            PlayerStats ps = FindFirstObjectByType<PlayerStats>();
            if (ps != null) ApplyUpgrades(ps);
        }

        // Press 2 to add speed level
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            speedLevel++;
            Debug.Log($"Speed level: {speedLevel}");
            PlayerDiveController dc = FindFirstObjectByType<PlayerDiveController>();
            if (dc != null) ApplySpeedUpgrade(dc);
        }

        // Press 3 to add oxygen level
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            oxygenLevel++;
            Debug.Log($"Oxygen level: {oxygenLevel}");
            PlayerStats ps = FindFirstObjectByType<PlayerStats>();
            if (ps != null) ApplyUpgrades(ps);
        }

        // Press 4 to add damage level
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            damageLevel++;
            Debug.Log($"Damage level: {damageLevel}");
        }

        // Press 0 to print current stats
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
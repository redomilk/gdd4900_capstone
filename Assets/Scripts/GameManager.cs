using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int scrapCount = 20;
    public int healthLevel = 0;
    public int speedLevel = 0;
    public int oxygenLevel = 0;
    public int damageLevel = 0;
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
        PauseMenu pauseMenu = FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null)
        {
            // Find PauseMenu object then get PausePanel as child
            GameObject pauseMenuObj = GameObject.Find("PauseMenu");
            if (pauseMenuObj != null)
                pauseMenu.pausePanel = pauseMenuObj.transform.Find("PausePanel").gameObject;
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
}
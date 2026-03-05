using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public GameObject optionsPanel;

    [Header("Things to disable while paused")]
    public MonoBehaviour[] disableThese;

    bool isPaused;
    PlayerInput playerInput;
    CanvasGroup pausePanelGroup;

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    // Called by GameManager after panels are rewired
    public void RewireButtons()
    {
        if (pausePanel == null) return;

        WireButton(pausePanel, "ResumeButton", Resume);
        WireButton(pausePanel, "OptionsButton", OpenOptions);
        WireButton(pausePanel, "RestartButton", RestartScene);
        WireButton(pausePanel, "QuitButton", QuitGame);

        if (optionsPanel != null)
        {
            // Force activate it we ca search inside it
            bool wasActive = optionsPanel.activeSelf;
            optionsPanel.SetActive(true);
            WireButton(optionsPanel, "BackButton", CloseOptions);
            optionsPanel.SetActive(wasActive);  // restore original state
        }

        pausePanelGroup = pausePanel.GetComponent<CanvasGroup>();
    }

    void WireButton(GameObject panel, string buttonName, UnityEngine.Events.UnityAction action)
    {
        // Search all children recursively instead of just direct children
        Transform t = FindDeepChild(panel.transform, buttonName);
        if (t == null)
        {
            Debug.LogWarning($"Button '{buttonName}' not found in {panel.name}");
            foreach (Transform child in panel.GetComponentsInChildren<Transform>(true))
                //Debug.Log($"  Child: '{child.name}'");
            return;
        }
        Button btn = t.GetComponent<Button>();
        if (btn == null)
        {
            //Debug.LogWarning($"No Button component on '{buttonName}'");
            return;
        }
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
       // Debug.Log($"Wired: {buttonName}");
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    void Start()
    {
        RewireButtons();
        Resume();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        bool qPressed = Keyboard.current.qKey.wasPressedThisFrame;

        if (escPressed || qPressed)
        {
            if (optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptions();
                return;
            }
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (playerInput != null) playerInput.enabled = false;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        else
            Debug.LogWarning("pausePanel is null on Pause!");
        Time.timeScale = 0f;
        isPaused = true;
        if (disableThese != null)
            foreach (var b in disableThese)
                if (b != null) b.enabled = false;
    }

    public void Resume()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (pausePanelGroup != null)
        {
            pausePanelGroup.interactable = true;
            pausePanelGroup.alpha = 1f;
        }
        Time.timeScale = 1f;
        isPaused = false;
        if (disableThese != null)
            foreach (var b in disableThese)
                if (b != null) b.enabled = true;
        if (playerInput != null) playerInput.enabled = true;
    }

    public void OpenOptions()
    {
        if (pausePanelGroup != null)
        {
            pausePanelGroup.interactable = false;
            pausePanelGroup.alpha = 0.4f;
        }
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (pausePanelGroup != null)
        {
            pausePanelGroup.interactable = true;
            pausePanelGroup.alpha = 1f;
        }
    }

    public void RestartScene()
    {
        Resume();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
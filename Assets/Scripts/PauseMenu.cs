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
    CanvasGroup pausePanelGroup;  // ADD THIS

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        pausePanelGroup = pausePanel.GetComponent<CanvasGroup>();
    }

    void Start()
    {
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
        // dims pause panel
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
        // restores pausePanel to full brightness
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
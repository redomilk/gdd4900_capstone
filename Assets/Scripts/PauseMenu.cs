using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Things to disable while paused")]
    public MonoBehaviour[] disableThese;

    bool isPaused;
    PlayerInput playerInput;

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>(); // or assign via inspector if you prefer
    }

    void Start()
    {
        Resume(); // clean start
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        bool qPressed = Keyboard.current.qKey.wasPressedThisFrame;

        if (escPressed || qPressed)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (playerInput != null) playerInput.enabled = false;

        if (pausePanel != null) pausePanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        if (disableThese != null)
            foreach (var b in disableThese)
                if (b != null) b.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (disableThese != null)
            foreach (var b in disableThese)
                if (b != null) b.enabled = true;

        if (playerInput != null) playerInput.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
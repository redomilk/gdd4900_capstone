using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject controlsPanel;

    [Header("Things to disable while paused")]
    public MonoBehaviour[] disableThese;

    [Header("Fade")]
    public float panelFadeDuration = 0.15f;

    bool isPaused;
    PlayerInput playerInput;

    CanvasGroup pausePanelGroup;
    CanvasGroup optionsPanelGroup;
    CanvasGroup controlsPanelGroup;

    Coroutine panelFadeRoutine;

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
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
        RebindPanels();
        RewireButtons();
        Resume();
    }

    void Start()
    {
        RebindPanels();
        RewireButtons();
        Resume();
    }

    void RebindPanels()
    {
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        pausePanel = null;
        optionsPanel = null;
        controlsPanel = null;

        foreach (Canvas canvas in allCanvases)
        {
            Transform[] allChildren = canvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == "PausePanel") pausePanel = t.gameObject;
                if (t.name == "OptionsPanel") optionsPanel = t.gameObject;
                if (t.name == "ControlsPanel") controlsPanel = t.gameObject;
            }
        }

        pausePanelGroup = GetOrAddCanvasGroup(pausePanel);
        optionsPanelGroup = GetOrAddCanvasGroup(optionsPanel);
        controlsPanelGroup = GetOrAddCanvasGroup(controlsPanel);
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject go)
    {
        if (go == null) return null;

        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = go.AddComponent<CanvasGroup>();

        return cg;
    }

    public void RewireButtons()
    {
        if (pausePanel == null) return;

        WireButton(pausePanel, "ResumeButton", Resume);
        WireButton(pausePanel, "OptionsButton", OpenOptions);
        WireButton(pausePanel, "ControlsButton", OpenControls);
        WireButton(pausePanel, "MainMenuButton", GoToMainMenu);
        WireButton(pausePanel, "QuitButton", QuitGame);

        if (optionsPanel != null)
        {
            WireButton(optionsPanel, "backButton", CloseOptions);
        }

        if (controlsPanel != null)
        {
            WireButton(controlsPanel, "backButton", CloseControls);
        }
    }

    void WireButton(GameObject panel, string buttonName, UnityEngine.Events.UnityAction action)
    {
        Transform t = FindDeepChild(panel.transform, buttonName);
        if (t == null)
        {
            Debug.LogWarning($"Button '{buttonName}' not found in {panel.name}");
            return;
        }

        Button btn = t.GetComponent<Button>();
        if (btn == null)
            return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        bool qPressed = Keyboard.current.qKey.wasPressedThisFrame;

        if (escPressed || qPressed)
        {
            if (controlsPanel != null && controlsPanel.activeSelf)
            {
                CloseControls();
                return;
            }

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
        {
            pausePanel.SetActive(true);
            SetPanelState(pausePanelGroup, true, true, 1f);
        }
        else
        {
            Debug.LogWarning("pausePanel is null on Pause!");
        }

        if (optionsPanel != null)
            SetPanelHidden(optionsPanel, optionsPanelGroup);

        if (controlsPanel != null)
            SetPanelHidden(controlsPanel, controlsPanelGroup);

        Time.timeScale = 0f;
        isPaused = true;

        if (disableThese != null)
            foreach (var b in disableThese)
                if (b != null) b.enabled = false;
    }

    public void Resume()
    {
        if (panelFadeRoutine != null)
        {
            StopCoroutine(panelFadeRoutine);
            panelFadeRoutine = null;
        }

        if (optionsPanel != null)
            SetPanelHidden(optionsPanel, optionsPanelGroup);

        if (controlsPanel != null)
            SetPanelHidden(controlsPanel, controlsPanelGroup);

        if (pausePanel != null)
            SetPanelHidden(pausePanel, pausePanelGroup);

        Time.timeScale = 1f;
        isPaused = false;

        if (disableThese != null)
            foreach (var b in disableThese)
                if (b != null) b.enabled = true;

        if (playerInput != null) playerInput.enabled = true;
    }

    public void OpenOptions()
    {
        if (pausePanel == null || optionsPanel == null) return;

        optionsPanel.SetActive(true);

        if (panelFadeRoutine != null)
            StopCoroutine(panelFadeRoutine);
        panelFadeRoutine = StartCoroutine(FadeBetweenPanels(
            pausePanelGroup, optionsPanelGroup,
            pausePanel, optionsPanel));
    }

    public void CloseOptions()
    {
        if (pausePanel == null || optionsPanel == null) return;

        pausePanel.SetActive(true);

        if (panelFadeRoutine != null)
            StopCoroutine(panelFadeRoutine);
        panelFadeRoutine = StartCoroutine(FadeBetweenPanels(
            optionsPanelGroup, pausePanelGroup,
            optionsPanel, pausePanel));
    }

    public void OpenControls()
    {
        if (pausePanel == null || controlsPanel == null) return;

        controlsPanel.SetActive(true);

        if (panelFadeRoutine != null)
            StopCoroutine(panelFadeRoutine);
        panelFadeRoutine = StartCoroutine(FadeBetweenPanels(
            pausePanelGroup, controlsPanelGroup,
            pausePanel, controlsPanel));
    }

    public void CloseControls()
    {
        if (pausePanel == null || controlsPanel == null) return;

        pausePanel.SetActive(true);

        if (panelFadeRoutine != null)
            StopCoroutine(panelFadeRoutine);
        panelFadeRoutine = StartCoroutine(FadeBetweenPanels(
            controlsPanelGroup, pausePanelGroup,
            controlsPanel, pausePanel));
    }

    IEnumerator FadeBetweenPanels(CanvasGroup from, CanvasGroup to, GameObject fromObject, GameObject toObject)
    {
        if (from == null || to == null)
            yield break;

        SetPanelState(to, false, false, 0f);
        if (toObject != null) toObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < panelFadeDuration)
        {
            float t = elapsed / panelFadeDuration;

            from.alpha = 1f - t;
            to.alpha = t;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        from.alpha = 0f;
        to.alpha = 1f;

        if (fromObject != null) fromObject.SetActive(false);

        SetPanelState(from, false, false, 0f);
        SetPanelState(to, true, true, 1f);

        panelFadeRoutine = null;
    }

    void SetPanelHidden(GameObject panelObject, CanvasGroup group)
    {
        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        if (panelObject != null)
            panelObject.SetActive(false);
    }

    void SetPanelState(CanvasGroup group, bool interactable, bool blocksRaycasts, float alpha)
    {
        if (group == null) return;

        group.interactable = interactable;
        group.blocksRaycasts = blocksRaycasts;
        group.alpha = alpha;
    }

    public void GoToMainMenu()
    {
        Resume();
        Time.timeScale = 1f;
        AudioManager.Instance.SetMusicState("Menu");
        SceneManager.LoadScene("Main Menu");
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
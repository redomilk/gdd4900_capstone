using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("FMOD Events")]
    [SerializeField] private EventReference musicEvent;

    private EventInstance musicInstance;
    private bool musicStarted;
    private string currentState = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create once while Bootstrap is fresh.
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
    }

    public void SetMusicState(string state)
    {
        if (!musicInstance.isValid())
        {
            Debug.LogError("Music instance is invalid.");
            return;
        }

        if (!musicStarted)
        {
            RuntimeManager.StudioSystem.flushCommands();

            musicInstance.start();
            musicStarted = true;
        }

        currentState = state;
        musicInstance.setParameterByNameWithLabel("GameState", state);

        Debug.Log("Music state set to: " + state);
    }

    private void OnDestroy()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }

        if (Instance == this)
            Instance = null;
    }

    public void RestartMusicInState(string state)
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.setParameterByNameWithLabel("GameState", state);
        musicInstance.start();

        musicStarted = true;
        currentState = state;
    }
}
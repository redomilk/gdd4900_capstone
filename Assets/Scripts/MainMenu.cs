using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject player;

    [Header("HUD Elements")]
    [SerializeField] private GameObject depthGauge;
    [SerializeField] private GameObject resourceBars;

    private bool gameStarted = false;

    void Start()
    {
        // Game begins on menu, hide HUD
        menuCanvas.SetActive(true);
        player.SetActive(false);
        depthGauge.SetActive(false);
        resourceBars.SetActive(false);
    }

    public void StartGame()
    {
        menuCanvas.SetActive(false);
        player.SetActive(true);
        depthGauge.SetActive(true);
        resourceBars.SetActive(true);
        gameStarted = true;
    }

    public void ReturnToMenu()
    {
        menuCanvas.SetActive(true);
        player.SetActive(false);
        depthGauge.SetActive(false);
        resourceBars.SetActive(false);
    }
}
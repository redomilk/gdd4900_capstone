using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject player;
    [Header("HUD Elements")]
    public GameObject depthGauge;
    public GameObject resourceBars;

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
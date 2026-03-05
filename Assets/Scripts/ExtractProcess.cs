using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ExtractProcess : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string hubSceneName = "HUB";
    private bool playerInZone = false;

    void Update()
    {
        if (playerInZone && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Extract();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInZone = true;
        if (PlayerHUD.instance != null)
            PlayerHUD.instance.ShowPrompt("Press E to escape");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInZone = false;
        if (PlayerHUD.instance != null)
            PlayerHUD.instance.HidePrompt();
    }

    private void Extract()
    {
        if (PlayerHUD.instance != null)
            PlayerHUD.instance.HidePrompt();
        GameManager.instance.BankRunScrap();
        SceneManager.LoadScene(hubSceneName);
    }
}
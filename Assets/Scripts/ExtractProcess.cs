using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ExtractProcess : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private bool playerInZone = false;
    bool _ExtractPromptFired;

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

        if (GameManager.instance != null)
            GameManager.instance.CompleteRunExtract();

        SceneManager.LoadScene("Extract Scene");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ExtractProcess : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private bool playerInZone = false;
    private static bool shownExtractTipThisSession = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetSessionFlag()
    {
        shownExtractTipThisSession = false;
    }

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

        if (!shownExtractTipThisSession && TooltipPopup.Instance != null)
        {
            shownExtractTipThisSession = true;

            TooltipPopup.Instance.Show(
                "Found an Extract!",
                "Escape now with your scrap, or keep diving for greater rewards."
            );
        }
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
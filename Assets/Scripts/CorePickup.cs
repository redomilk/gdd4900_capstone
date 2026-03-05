using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using TMPro;

public class CorePickup : MonoBehaviour
{
    [Header("New Light")]
    public float pointLightRadius = 6f;
    public float pointLightIntensity = 1.2f;
    public Color pointLightColor = new Color(0.4f, 0.8f, 1f);

    bool playerNearby;
    bool collected;
    bool collectQueued;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            collectQueued = true;

        if (collected) return;

        if (playerNearby && collectQueued)
        {
            collectQueued = false;
            Collect();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (PlayerHUD.instance != null)
            PlayerHUD.instance.ShowPrompt("Press E to collect");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (PlayerHUD.instance != null)
            PlayerHUD.instance.HidePrompt();
    }

    void Collect()
    {
        collected = true;
        if (PlayerHUD.instance != null)
            PlayerHUD.instance.HidePrompt();

        // Re-find player in case cached reference went stale
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Debug.Log("Player found, upgrading light");
            UpgradeLight(player);
        }
        else Debug.LogWarning("Player is null in Collect!");

        TooltipPopup.Instance.Show("Flashlight Augment",
        "Augment collected Flashlight has been upgraded.\n\n" +
        "Press F to toggle it on and off.\n\n" +
        "See more Dive deeper.");

        Destroy(gameObject);
    }

    void UpgradeLight(GameObject player)
    {
        Flashlight flashlightScript = player.GetComponent<Flashlight>();
        if (flashlightScript == null || flashlightScript.flashlight == null) return;
        Light2D light = flashlightScript.flashlight;
        light.lightType = Light2D.LightType.Point;
        light.pointLightOuterRadius = pointLightRadius;
        light.pointLightInnerRadius = pointLightRadius * 0.4f;
        light.intensity = pointLightIntensity;
        light.color = pointLightColor;
        light.transform.SetParent(player.transform);
        light.transform.localPosition = Vector3.zero;
        light.transform.localRotation = Quaternion.identity;
        light.enabled = false;
    }
}
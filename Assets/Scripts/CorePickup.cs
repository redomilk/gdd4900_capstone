using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using TMPro;

public class CorePickup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro promptText;

    [Header("New Light")]
    public float pointLightRadius = 6f;
    public float pointLightIntensity = 1.2f;
    public Color pointLightColor = new Color(0.4f, 0.8f, 1f);

    bool playerNearby;
    bool collected;
    bool collectQueued;

    void Start()
    {
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            collectQueued = true;
            Debug.Log($"CorePickup: E queued, nearby:{playerNearby}, collected:{collected}");
        }

        if (collected) return;

        if (playerNearby && collectQueued)
        {
            Debug.Log("CorePickup: Calling Collect!");
            collectQueued = false;
            Collect();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (promptText != null) promptText.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    void Collect()
    {
        collected = true;
        if (promptText != null) promptText.gameObject.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) UpgradeLight(player);

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

        // Also detach from player rotation so it stays circular
        light.transform.SetParent(player.transform);
        light.transform.localPosition = Vector3.zero;
        light.transform.localRotation = Quaternion.identity;

        light.enabled = false;
    }
}
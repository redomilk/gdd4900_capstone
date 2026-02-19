using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public Light2D flashlight;

    void Start()
    {
        if (flashlight == null)
            flashlight = GetComponentInChildren<Light2D>();
        flashlight.enabled = false;
    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
            flashlight.enabled = !flashlight.enabled;
    }
}
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DepthLighting : MonoBehaviour
{
    [Header("Depth Reference")]
    public Transform depthMarker;

    [Header("Depth Range")]
    public float shallowDepth = 0f;
    public float deepDepth = -50f;

    [Header("Light Settings")]
    public Color shallowColor = new Color(0.6f, 0.8f, 1f);
    public Color deepColor = new Color(0f, 0.02f, 0.08f);
    [Range(0f, 1f)] public float shallowIntensity = 1.5f;
    [Range(0f, 1f)] public float deepIntensity = 0.05f;

    Light2D globalLight;

    void Awake()
    {
        globalLight = GetComponent<Light2D>();
    }

    void Update()
    {
        if (depthMarker == null || globalLight == null) return;

        float t = Mathf.Clamp01(Mathf.InverseLerp(shallowDepth, deepDepth, depthMarker.position.y));

        globalLight.color = Color.Lerp(shallowColor, deepColor, t);
        globalLight.intensity = Mathf.Lerp(shallowIntensity, deepIntensity, t);
    }

    void Start()
    {
        Debug.Log($"Starting Y: {depthMarker.position.y}");
        Debug.Log($"Starting t: {Mathf.Clamp01(Mathf.InverseLerp(shallowDepth, deepDepth, depthMarker.position.y))}");
    }
}
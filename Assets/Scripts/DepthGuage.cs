using UnityEngine;
using UnityEngine.UI;

public class DepthGauge : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform depthMarker;
    public Slider depthSlider;

    float startingDepth;

    void Start()
    {
        startingDepth = Mathf.Abs(player.position.y - depthMarker.position.y);
        depthSlider.value = 0f;
    }

    void Update()
    {
        float currentDepth = Mathf.Abs(player.position.y - depthMarker.position.y);

        float normalized = 1f - Mathf.Clamp01(currentDepth / startingDepth);

        depthSlider.value = normalized;
    }
}
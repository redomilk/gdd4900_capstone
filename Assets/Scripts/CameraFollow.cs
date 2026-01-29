using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Follow Settings")]
    public float smoothTime = 0.2f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Deadzone (optional)")]
    public float deadzoneRadius = 0.1f;

    Vector3 velocity;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = target.position + offset;

        // Deadzone so tiny player wiggles don't move camera
        float dist = Vector2.Distance(transform.position, targetPos);
        if (dist < deadzoneRadius) return;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
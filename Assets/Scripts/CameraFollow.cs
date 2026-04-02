using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [Header("Follow Settings")]
    public float smoothTime = 0.2f;
    public Vector3 offset = new Vector3(0, 0, -10);
    [Header("Deadzone (optional)")]
    public float deadzoneRadius = 0.1f;
    [Header("Look Ahead")]
    public float lookAheadAmount = 1.5f; 
    public float lookAheadSmoothTime = 0.3f;

    Vector3 velocity;
    Vector3 lookAheadVelocity;
    Vector3 currentLookAhead;
    Vector3 lastTargetPos;

    void LateUpdate()
    {
        if (!target) return;

        // Calculate look-ahead based on player movement direction
        Vector3 moveDir = (target.position - lastTargetPos) / Time.deltaTime;
        moveDir.z = 0f;
        Vector3 targetLookAhead = moveDir.normalized * Mathf.Min(moveDir.magnitude, lookAheadAmount);
        currentLookAhead = Vector3.SmoothDamp(currentLookAhead, targetLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);

        lastTargetPos = target.position;

        Vector3 targetPos = target.position + offset + currentLookAhead;

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
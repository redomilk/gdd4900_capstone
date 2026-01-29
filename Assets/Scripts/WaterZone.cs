using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private void Reset()
    {
        var c = GetComponent<BoxCollider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("ENTER WATER");
        GameEvents.OnPlayerHitWater?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("EXIT WATER");
        GameEvents.OnPlayerLeftWater?.Invoke();
    }
}
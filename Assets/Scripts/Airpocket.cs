using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AirPocket : MonoBehaviour
{
    void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameEvents.OnPlayerEnterAirPocket?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        GameEvents.OnPlayerExitAirPocket?.Invoke();
    }
}
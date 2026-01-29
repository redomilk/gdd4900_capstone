using UnityEngine;

public class OxygenBubblePickup : MonoBehaviour
{
    public float oxygenRestore = 30f;
    public float floatSpeed = 0.5f; // gentle upward drift

    void Update()
    {
        // cute little float effect
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player == null) return;

        player.ModifyOxygen(oxygenRestore);

        Destroy(gameObject);
    }
}
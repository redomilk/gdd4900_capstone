using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public float damagePerSecond = 10f;

    PlayerStats playerInside;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
            playerInside = player;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null && player == playerInside)
            playerInside = null;
    }

    void Update()
    {
        if (playerInside == null) return;

        // smooth damage over time
        playerInside.TakeDamage(damagePerSecond * Time.deltaTime);
    }
}

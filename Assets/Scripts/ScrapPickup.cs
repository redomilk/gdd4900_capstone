using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    public int scrapValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.scrapCount += scrapValue;
            Destroy(gameObject);
        }
    }
}
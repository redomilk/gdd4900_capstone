using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    public int scrapValue = 1;
    public float fallSpeed = 0.5f;
    public float rayDistance = 0.5f;
    public LayerMask wallLayer; // assign this in Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.scrapCount += scrapValue;
            GameManager.instance.scrapCount += scrapValue;
            Destroy(gameObject);
        }
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, wallLayer);

        if (hit.collider == null)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
    }
}
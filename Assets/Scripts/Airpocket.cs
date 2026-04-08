using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class AirPocket : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private float shrinkSpeed = 0.5f;
    [SerializeField] private float regrowSpeed = 0.3f;
    [SerializeField] private float minSize = 0.05f;

    [Header("Ambient Bubbles (always on)")]
    [SerializeField] private GameObject miniBubblePrefab;
    [SerializeField] private float ambientBubbleSpawnInterval = 0.2f;
    [SerializeField] private int ambientBubblesPerSpawn = 1;
    [SerializeField] private float ambientSpawnRadius = 0.45f;
    [SerializeField] private float ambientBubbleScaleMin = 0.18f;
    [SerializeField] private float ambientBubbleScaleMax = 0.3f;

    [Header("Consumption Bubbles (player inside)")]
    [SerializeField] private float consumeBubbleSpawnInterval = 0.08f;
    [SerializeField] private int consumeBubblesPerSpawn = 2;
    [SerializeField] private float consumeSpawnRadius = 0.35f;
    [SerializeField] private float consumeBubbleScaleMin = 0.08f;
    [SerializeField] private float consumeBubbleScaleMax = 0.16f;

    private bool playerInside = false;
    private SpriteRenderer sr;
    private float startSize;

    private float ambientBubbleTimer;
    private float consumeBubbleTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startSize = transform.localScale.x;
    }

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void Update()
    {
        HandleAmbientBubbleSpawning();

        Vector3 newScale = transform.localScale;

        if (playerInside)
        {
            newScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
            HandleConsumeBubbleSpawning();
        }
        else
        {
            newScale += Vector3.one * regrowSpeed * Time.deltaTime;
            consumeBubbleTimer = 0f;
        }

        newScale = Vector3.Max(newScale, Vector3.zero);
        newScale = Vector3.Min(newScale, Vector3.one * startSize);

        transform.localScale = newScale;

        float normalizedSize = Mathf.InverseLerp(0f, startSize, newScale.x);
        Color c = sr.color;
        c.a = normalizedSize;
        sr.color = c;

        if (newScale.x <= minSize)
        {
            Destroy(gameObject);
        }
    }

    private void HandleAmbientBubbleSpawning()
    {
        if (miniBubblePrefab == null) return;

        ambientBubbleTimer += Time.deltaTime;

        while (ambientBubbleTimer >= ambientBubbleSpawnInterval)
        {
            ambientBubbleTimer -= ambientBubbleSpawnInterval;

            for (int i = 0; i < ambientBubblesPerSpawn; i++)
            {
                SpawnBubble(ambientSpawnRadius, ambientBubbleScaleMin, ambientBubbleScaleMax);
            }
        }
    }

    private void HandleConsumeBubbleSpawning()
    {
        if (miniBubblePrefab == null) return;

        consumeBubbleTimer += Time.deltaTime;

        while (consumeBubbleTimer >= consumeBubbleSpawnInterval)
        {
            consumeBubbleTimer -= consumeBubbleSpawnInterval;

            for (int i = 0; i < consumeBubblesPerSpawn; i++)
            {
                SpawnBubble(consumeSpawnRadius, consumeBubbleScaleMin, consumeBubbleScaleMax);
            }
        }
    }

    private void SpawnBubble(float spawnRadius, float scaleMin, float scaleMax)
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadius * transform.localScale.x;
        Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);

        GameObject bubble = Instantiate(miniBubblePrefab, spawnPos, Quaternion.identity);

        float randomScale = Random.Range(scaleMin, scaleMax);
        bubble.transform.localScale = Vector3.one * randomScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        GameEvents.OnPlayerEnterAirPocket?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        GameEvents.OnPlayerExitAirPocket?.Invoke();
    }
}
using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 4f;

    [Header("Tracer")]
    public float tracerWidth = 0.05f;
    public float tracerTime = 0.15f;
    public Color tracerColor = new Color(0f, 1f, 0.2f, 1f);  // green

    void Start()
    {
        Destroy(gameObject, lifetime);
        SetupTracer();
    }

    void SetupTracer()
    {
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = tracerTime;
        trail.startWidth = tracerWidth;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));

        // Fade from full green to transparent
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(tracerColor, 0f),
                new GradientColorKey(tracerColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = gradient;
        trail.sortingLayerName = "Default";  // change to match your bullet's sorting layer
        trail.sortingOrder = 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.TakeDamageWithKnockback(damage, transform.position); 
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            Destroy(gameObject);
    }
}
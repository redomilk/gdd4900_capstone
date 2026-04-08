using UnityEngine;

public class AirPocketBubble : MonoBehaviour
{
    [SerializeField] private float minLifetime = 0.25f;
    [SerializeField] private float maxLifetime = 0.6f;
    [SerializeField] private float minRiseSpeed = 0.3f;
    [SerializeField] private float maxRiseSpeed = 1.0f;
    [SerializeField] private float driftAmount = 0.35f;

    private SpriteRenderer sr;
    private float lifetime;
    private float age;
    private Vector3 moveDir;
    private Color startColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        lifetime = Random.Range(minLifetime, maxLifetime);

        float riseSpeed = Random.Range(minRiseSpeed, maxRiseSpeed);
        float driftX = Random.Range(-driftAmount, driftAmount);
        moveDir = new Vector3(driftX, riseSpeed, 0f);

        startColor = sr.color;
    }

    void Update()
    {
        age += Time.deltaTime;

        transform.position += moveDir * Time.deltaTime;

        float t = age / lifetime;

        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t);
        sr.color = c;

        if (age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
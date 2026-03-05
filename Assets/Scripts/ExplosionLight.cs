using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExplosionLight : MonoBehaviour
{
    public float duration = 0.2f;
    public float startIntensity = 2f;

    float timer;
    Light2D lightComp;

    void Start()
    {
        lightComp = GetComponent<Light2D>();
        lightComp.intensity = startIntensity;
    }

    void Update()
    {
        timer += Time.deltaTime;

        lightComp.intensity = Mathf.Lerp(startIntensity, 0, timer / duration);

        if (timer >= duration)
            Destroy(gameObject);
    }
}
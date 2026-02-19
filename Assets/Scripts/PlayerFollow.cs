using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float stopDistance = 0.5f;
    public float followDistance = 5f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 currentPos = transform.position;
        Vector2 targetPos = player.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance < followDistance && distance > stopDistance)
        {
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }
}
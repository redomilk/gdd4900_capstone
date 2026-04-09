using UnityEngine;

public class BossSpawnedEnemy : MonoBehaviour
{
    public BossController boss;

    void OnDestroy()
    {
        if (boss != null)
            boss.OnSpawnedEnemyDied();
    }
}
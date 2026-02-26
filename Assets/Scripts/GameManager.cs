using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int scrapCount = 20;
    public int healthLevel = 0;
    public int speedLevel = 0;
    public int oxygenLevel = 0;
    public int damageLevel = 0;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (instance != null) return;
        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
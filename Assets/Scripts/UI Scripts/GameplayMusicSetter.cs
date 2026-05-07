using UnityEngine;

public class GameplayMusicSetter : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.SetMusicState("Gameplay");
    }
}
using UnityEngine;

public class MainMenuMusicSetter : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager missing in Main Menu");
            return;
        }

        //AudioManager.Instance.SetMusicState("Menu");
        AudioManager.Instance.RestartMusicInState("Menu");
    }
}
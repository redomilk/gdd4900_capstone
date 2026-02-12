using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    [Tooltip("Where the click point is inside the texture")]
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        Cursor.visible = true;
    }
}
using UnityEngine;

public class cursor_dan : MonoBehaviour {

    public Texture2D[] cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        SetCursorDefault();
    }

    //0 = default
    //1 = move ready
    //2 = move not ready

    public void SetCursorDefault()
    {
        Cursor.SetCursor(cursorTexture[0], hotSpot, cursorMode);
    }

    public void SetCursorMoveReady()
    {
        Cursor.SetCursor(cursorTexture[1], hotSpot, cursorMode);
    }

    public void SetCursorMoveNotReady()
    {
        Cursor.SetCursor(cursorTexture[2], hotSpot, cursorMode);
    }

    public void SetCursorAttackReady()
    {
        Cursor.SetCursor(cursorTexture[3], hotSpot, cursorMode);
    }

    public void SetCursorAttackNotReady()
    {
        Cursor.SetCursor(cursorTexture[4], hotSpot, cursorMode);
    }
}

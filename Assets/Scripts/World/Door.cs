using UnityEngine;

public class Door : MonoBehaviour
{
    public DoorDirection direction;

    private bool isLocked = false;
    private Collider2D col;
    public SpriteRenderer sr;

    // Colors
    private static readonly Color unlockedColor = new Color(0.75f, 0.75f, 0.75f);
    private static readonly Color lockedColor = Color.gray;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        SetLocked(false); // default state
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (col != null)
            col.enabled = !locked;

        if (sr != null)
            sr.color = locked ? lockedColor : unlockedColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLocked)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (RoomManager.Instance.IsTransitioning)
            return;

        RoomManager.Instance.MoveToRoom(direction);
    }
}

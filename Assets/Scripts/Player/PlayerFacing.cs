using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public int FacingDirection { get; private set; } = 1;
    public bool IsShooting { get; private set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Face(float x, bool force = false)
    {
        if (x == 0) return;
        if (IsShooting && !force) return;

        FacingDirection = x > 0 ? 1 : -1;
        spriteRenderer.flipX = FacingDirection == -1;
    }

    public void LockFacingForShot(float x)
    {
        IsShooting = true;
        Face(x, true);
    }

    public void UnlockFacing()
    {
        IsShooting = false;
    }
}

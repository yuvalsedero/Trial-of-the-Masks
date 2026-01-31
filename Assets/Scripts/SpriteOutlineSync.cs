using UnityEngine;

public class SpriteOutlineSync : MonoBehaviour
{
    public SpriteRenderer mainSprite;
    public SpriteRenderer outlineSprite;

    void LateUpdate()
    {
        if (mainSprite == null || outlineSprite == null)
            return;

        outlineSprite.sprite = mainSprite.sprite;
        outlineSprite.flipX = mainSprite.flipX;
    }
}

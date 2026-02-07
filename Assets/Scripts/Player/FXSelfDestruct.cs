using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FXSelfDestruct : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            // Destroy after the length of the default animation
            float duration = anim.runtimeAnimatorController.animationClips[0].length;
            Destroy(gameObject, duration);
        }
        else
        {
            Destroy(gameObject, 2f); // fallback
        }
    }

    // Optional Animation Event still works
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

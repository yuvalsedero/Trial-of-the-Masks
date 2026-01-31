using UnityEngine;

public class BearBossThrow : MonoBehaviour
{
    public GameObject stonePrefab;
    public Transform throwPoint;

    Animator anim;
    Transform player;

    [Header("Throw Sounds")]
public AudioClip[] throwSounds;
private AudioSource audioSource;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
if (audioSource == null)
    audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // called by controller
    public void StartThrow()
    {
        Debug.Log("Boss StartThrow called");
        if (anim != null)
            anim.SetTrigger("Attack");
    }

    // called by ANIMATION EVENT
    public void SpawnStone()
    {
        if (stonePrefab == null || throwPoint == null || player == null)
            return;

        Vector2 dir = (player.position - throwPoint.position).normalized;

        GameObject stone = Instantiate(
            stonePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        // Play random throw sound
if (throwSounds != null && throwSounds.Length > 0)
{
    AudioClip clip = throwSounds[Random.Range(0, throwSounds.Length)];
    audioSource.PlayOneShot(clip);
}

        StoneProjectile proj = stone.GetComponent<StoneProjectile>();
        if (proj != null)
            proj.Launch(dir);
    }
}

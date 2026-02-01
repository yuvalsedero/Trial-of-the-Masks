using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        MusicManager.Instance.EnterBoss();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        MusicManager.Instance.ExitBoss();
    }

    void OnDisable()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.ExitBoss();
    }
}
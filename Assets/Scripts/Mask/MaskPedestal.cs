using UnityEngine;

public enum MaskCategory
{
    Elemental,
    Dart,
    Stat
}

public class MaskPedestal : MonoBehaviour
{
    public MaskEffectType effectType;
    public MaskCategory category;
    public TribeChief ownerChief;
    public AudioClip deathLoopSound;   // loop while dying
    public AudioClip deathImpactSound; // final one-shot on collision
    private AudioSource deathAudioSource; // for loop

    public void StartLoop()
{
    if (deathLoopSound == null) return;

    if (deathAudioSource == null)
    {
        GameObject tempAudio = new GameObject("MaskLoopAudio");
        deathAudioSource = tempAudio.AddComponent<AudioSource>();
        deathAudioSource.loop = true;
        deathAudioSource.clip = deathLoopSound;
        deathAudioSource.Play();
        DontDestroyOnLoad(tempAudio); // survives even if pedestal is destroyed
    }
}

void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player"))
        return;

    // Stop loop if playing
    if (deathAudioSource != null)
    {
        deathAudioSource.Stop();
        Destroy(deathAudioSource.gameObject, 0.1f); // cleanup loop object
    }

    // Play one-shot impact sound
    if (deathImpactSound != null)
    {
        GameObject tempAudio = new GameObject("MaskImpactAudio");
        AudioSource aSource = tempAudio.AddComponent<AudioSource>();
        aSource.clip = deathImpactSound;
        aSource.Play();
        Destroy(tempAudio, deathImpactSound.length);
    }

    // Apply mask effect
    switch (category)
    {
        case MaskCategory.Elemental:
            MaskManager.Instance.SetElemental(effectType);
            break;

        case MaskCategory.Dart:
            MaskManager.Instance.SetDart(effectType);
            break;

        case MaskCategory.Stat:
            MaskManager.Instance.ApplyStat(effectType);
            break;
    }

    ownerChief.OnMaskChosen(this);
    MaskManager.Instance.CollectMaskPiece();
    Destroy(gameObject);
}
}

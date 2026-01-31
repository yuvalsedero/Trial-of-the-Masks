using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Spawn (Intro)")]
    public AudioSource spawnMusic;
    public AudioSource spawnAmbience;

    [Header("Main")]
    public AudioSource mainMusic;
    public AudioSource mainAmbience;

    [Header("Tribe Layers")]
    public AudioSource tribeA;
    public AudioSource tribeB;
    public AudioSource tribeC;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;

    // Target volumes (your spec)
    const float MAIN_MUSIC_FULL = 1f;
    const float MAIN_MUSIC_TRIBE = 0.8f;
    const float MAIN_AMBIENCE = 0.8f;

    bool hasLeftSpawn = false;
    bool spawnFadeInProgress = false;

    Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Start EVERYTHING so layers stay synced
        spawnMusic.Play();
        spawnAmbience.Play();

        mainMusic.Play();
        mainAmbience.Play();

        tribeA.Play();
        tribeB.Play();
        tribeC.Play();

        // Initial volumes
        spawnMusic.volume = 1f;
        spawnAmbience.volume = 1f;

        mainMusic.volume = 0f;
        mainAmbience.volume = 0f;

        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;
    }

    // ===============================
    // SPAWN EXIT (ONE WAY)
    // ===============================
    public void LeaveSpawnForever()
    {
        if (hasLeftSpawn)
            return;

        hasLeftSpawn = true;
        spawnFadeInProgress = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeSpawnOutAndMainIn());
    }

    IEnumerator FadeSpawnOutAndMainIn()
    {
        float t = 0f;

        float sm0 = spawnMusic.volume;
        float sa0 = spawnAmbience.volume;

        float mm0 = mainMusic.volume;
        float ma0 = mainAmbience.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);

            spawnMusic.volume = Mathf.Lerp(sm0, 0f, k);
            spawnAmbience.volume = Mathf.Lerp(sa0, 0f, k);

            mainMusic.volume = Mathf.Lerp(mm0, MAIN_MUSIC_FULL, k);
            mainAmbience.volume = Mathf.Lerp(ma0, MAIN_AMBIENCE, k);

            yield return null;
        }

        // Snap final values
        spawnMusic.volume = 0f;
        spawnAmbience.volume = 0f;

        mainMusic.volume = MAIN_MUSIC_FULL;
        mainAmbience.volume = MAIN_AMBIENCE;

        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;

        spawnFadeInProgress = false;
    }

    // ===============================
    // TRIBE ROOMS
    // ===============================
    public void EnterTribe(TribeType tribe)
    {
        if (!hasLeftSpawn || spawnFadeInProgress)
            return;

        switch (tribe)
        {
            case TribeType.TribeA:
                StartFade(FadeToTribe(tribeA));
                break;
            case TribeType.TribeB:
                StartFade(FadeToTribe(tribeB));
                break;
            case TribeType.TribeC:
                StartFade(FadeToTribe(tribeC));
                break;
        }
    }

    public void ExitTribe()
    {
        if (!hasLeftSpawn || spawnFadeInProgress)
            return;

        StartFade(FadeToMain());
    }

    IEnumerator FadeToTribe(AudioSource tribe)
    {
        float t = 0f;

        float mm0 = mainMusic.volume;
        float ma0 = mainAmbience.volume;

        float ta0 = tribeA.volume;
        float tb0 = tribeB.volume;
        float tc0 = tribeC.volume;

        float targetA = (tribe == tribeA) ? 1f : 0f;
        float targetB = (tribe == tribeB) ? 1f : 0f;
        float targetC = (tribe == tribeC) ? 1f : 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);

            mainMusic.volume = Mathf.Lerp(mm0, MAIN_MUSIC_TRIBE, k);
            mainAmbience.volume = Mathf.Lerp(ma0, MAIN_AMBIENCE, k);

            tribeA.volume = Mathf.Lerp(ta0, targetA, k);
            tribeB.volume = Mathf.Lerp(tb0, targetB, k);
            tribeC.volume = Mathf.Lerp(tc0, targetC, k);

            yield return null;
        }

        mainMusic.volume = MAIN_MUSIC_TRIBE;
        mainAmbience.volume = MAIN_AMBIENCE;

        tribeA.volume = targetA;
        tribeB.volume = targetB;
        tribeC.volume = targetC;
    }

    IEnumerator FadeToMain()
    {
        float t = 0f;

        float mm0 = mainMusic.volume;
        float ma0 = mainAmbience.volume;

        float ta0 = tribeA.volume;
        float tb0 = tribeB.volume;
        float tc0 = tribeC.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);

            mainMusic.volume = Mathf.Lerp(mm0, MAIN_MUSIC_FULL, k);
            mainAmbience.volume = Mathf.Lerp(ma0, MAIN_AMBIENCE, k);

            tribeA.volume = Mathf.Lerp(ta0, 0f, k);
            tribeB.volume = Mathf.Lerp(tb0, 0f, k);
            tribeC.volume = Mathf.Lerp(tc0, 0f, k);

            yield return null;
        }

        mainMusic.volume = MAIN_MUSIC_FULL;
        mainAmbience.volume = MAIN_AMBIENCE;

        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;
    }

    // ===============================
    // FADE CONTROL
    // ===============================
    void StartFade(IEnumerator routine)
    {
        if (spawnFadeInProgress)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(routine);
    }
}

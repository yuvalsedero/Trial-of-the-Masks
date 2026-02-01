using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Spawn")]
    public AudioSource spawnMusic;
    public AudioSource spawnAmbience;

    [Header("Main")]
    public AudioSource mainMusic;
    public AudioSource mainAmbience;

    [Header("Tribes")]
    public AudioSource tribeA;
    public AudioSource tribeB;
    public AudioSource tribeC;

    [Header("Boss")]
    public AudioSource bossMusic;   // LOOPED

    [Header("Fade")]
    public float fadeDuration = 2f;

    const float MAIN_FULL = 1f;
    const float MAIN_TRIBE = 0.8f;
    const float AMBIENCE = 0.8f;

    bool hasLeftSpawn = false;
    bool spawnFadeInProgress = false;

    Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        spawnMusic.Play();
        spawnAmbience.Play();

        mainMusic.Play();
        mainAmbience.Play();

        tribeA.Play();
        tribeB.Play();
        tribeC.Play();

        bossMusic.Play();

        spawnMusic.volume = 1f;
        spawnAmbience.volume = 1f;

        mainMusic.volume = 0f;
        mainAmbience.volume = 0f;

        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;

        bossMusic.volume = 0f;
    }

    // ================= SPAWN =================
    public void LeaveSpawnForever()
    {
        if (hasLeftSpawn) return;

        hasLeftSpawn = true;
        spawnFadeInProgress = true;
        StartFade(FadeSpawnOutAndMainIn());
    }

    IEnumerator FadeSpawnOutAndMainIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            spawnMusic.volume = Mathf.Lerp(1f, 0f, k);
            spawnAmbience.volume = Mathf.Lerp(1f, 0f, k);

            mainMusic.volume = Mathf.Lerp(0f, MAIN_FULL, k);
            mainAmbience.volume = Mathf.Lerp(0f, AMBIENCE, k);

            yield return null;
        }

        spawnMusic.volume = 0f;
        spawnAmbience.volume = 0f;

        mainMusic.volume = MAIN_FULL;
        mainAmbience.volume = AMBIENCE;

        spawnFadeInProgress = false;
    }

    // ================= TRIBES =================
    public void EnterTribe(TribeType tribe)
    {
        StartFade(FadeToTribe(tribe));
    }

    public void ExitTribe()
    {
        if (!hasLeftSpawn || spawnFadeInProgress)
            return;

        StartFade(FadeToMain());
    }

    IEnumerator FadeToTribe(TribeType tribe)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            mainMusic.volume = Mathf.Lerp(mainMusic.volume, MAIN_TRIBE, k);

            tribeA.volume = Mathf.Lerp(tribeA.volume, tribe == TribeType.TribeA ? 1f : 0f, k);
            tribeB.volume = Mathf.Lerp(tribeB.volume, tribe == TribeType.TribeB ? 1f : 0f, k);
            tribeC.volume = Mathf.Lerp(tribeC.volume, tribe == TribeType.TribeC ? 1f : 0f, k);

            bossMusic.volume = Mathf.Lerp(bossMusic.volume, 0f, k);

            yield return null;
        }
    }

    IEnumerator FadeToMain()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            mainMusic.volume = Mathf.Lerp(mainMusic.volume, MAIN_FULL, k);

            tribeA.volume = Mathf.Lerp(tribeA.volume, 0f, k);
            tribeB.volume = Mathf.Lerp(tribeB.volume, 0f, k);
            tribeC.volume = Mathf.Lerp(tribeC.volume, 0f, k);

            bossMusic.volume = Mathf.Lerp(bossMusic.volume, 0f, k);

            yield return null;
        }
    }

    // ================= BOSS =================
    public void EnterBoss()
    {
        StartFade(FadeToBoss());
    }


    public void ExitBoss()
    {
        StartFade(FadeBossToMain());
    }

    IEnumerator FadeToBoss()
    {
        float t = 0f;

        float sm = spawnMusic.volume;
        float sa = spawnAmbience.volume;

        float mm = mainMusic.volume;
        float ma = mainAmbience.volume;

        float ta = tribeA.volume;
        float tb = tribeB.volume;
        float tc = tribeC.volume;

        float bm = bossMusic.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            // 🔥 KILL SPAWN
            spawnMusic.volume = Mathf.Lerp(sm, 0f, k);
            spawnAmbience.volume = Mathf.Lerp(sa, 0f, k);

            // 🔥 KILL MAIN
            mainMusic.volume = Mathf.Lerp(mm, 0f, k);
            mainAmbience.volume = Mathf.Lerp(ma, 0f, k);

            // 🔥 KILL TRIBES
            tribeA.volume = Mathf.Lerp(ta, 0f, k);
            tribeB.volume = Mathf.Lerp(tb, 0f, k);
            tribeC.volume = Mathf.Lerp(tc, 0f, k);

            // 🔥 BRING BOSS
            bossMusic.volume = Mathf.Lerp(bm, 1f, k);

            yield return null;
        }

        // SNAP FINAL
        spawnMusic.volume = 0f;
        spawnAmbience.volume = 0f;

        mainMusic.volume = 0f;
        mainAmbience.volume = 0f;

        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;

        bossMusic.volume = 1f;
    }


    IEnumerator FadeBossToMain()
    {
        float t = 0f;

        float bm = bossMusic.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            bossMusic.volume = Mathf.Lerp(bm, 0f, k);

            mainMusic.volume = Mathf.Lerp(0f, MAIN_FULL, k);
            mainAmbience.volume = Mathf.Lerp(0f, AMBIENCE, k);

            yield return null;
        }

        bossMusic.volume = 0f;
        mainMusic.volume = MAIN_FULL;
        mainAmbience.volume = AMBIENCE;
    }


    // ================= FADE CONTROL =================
    void StartFade(IEnumerator routine)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(routine);
    }
}

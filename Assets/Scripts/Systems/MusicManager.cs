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
    public AudioSource bossMusic;
    public AudioSource bossStinger; // one-shot stinger

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    const float MAIN_FULL = 0.8f;
    const float MAIN_TRIBE = 0.6f;
    const float AMBIENCE = 0.6f;

    bool hasLeftSpawn = false;
    bool spawnFadeInProgress = false;
    bool inBoss = false;

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

        spawnMusic.volume = 0.8f;
        spawnAmbience.volume = 0.5f;

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

        StartFade(FadeSpawnOutAndMainIn(), true);
    }

    IEnumerator FadeSpawnOutAndMainIn()
    {
        float t = 0f;

        float sm = spawnMusic.volume;
        float sa = spawnAmbience.volume;
        float mm = mainMusic.volume;
        float ma = mainAmbience.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            spawnMusic.volume = Mathf.Lerp(sm, 0f, k);
            spawnAmbience.volume = Mathf.Lerp(sa, 0f, k);

            mainMusic.volume = Mathf.Lerp(mm, MAIN_FULL, k);
            mainAmbience.volume = Mathf.Lerp(ma, AMBIENCE, k);

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
        if (!hasLeftSpawn || spawnFadeInProgress || inBoss)
            return;

        StartFade(FadeToTribe(tribe));
    }

    public void ExitTribe()
    {
        if (!hasLeftSpawn || spawnFadeInProgress || inBoss)
            return;

        StartFade(FadeToMain());
    }

    IEnumerator FadeToTribe(TribeType tribe)
    {
        float t = 0f;

        float mm = mainMusic.volume;
        float ta = tribeA.volume;
        float tb = tribeB.volume;
        float tc = tribeC.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            spawnMusic.volume = 0f;
            spawnAmbience.volume = 0f;

            mainMusic.volume = Mathf.Lerp(mm, MAIN_TRIBE, k);

            tribeA.volume = Mathf.Lerp(ta, tribe == TribeType.TribeA ? 1f : 0f, k);
            tribeB.volume = Mathf.Lerp(tb, tribe == TribeType.TribeB ? 1f : 0f, k);
            tribeC.volume = Mathf.Lerp(tc, tribe == TribeType.TribeC ? 1f : 0f, k);

            yield return null;
        }
    }

    IEnumerator FadeToMain()
    {
        float t = 0f;

        float mm = mainMusic.volume;
        float ta = tribeA.volume;
        float tb = tribeB.volume;
        float tc = tribeC.volume;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;

            mainMusic.volume = Mathf.Lerp(mm, MAIN_FULL, k);

            tribeA.volume = Mathf.Lerp(ta, 0f, k);
            tribeB.volume = Mathf.Lerp(tb, 0f, k);
            tribeC.volume = Mathf.Lerp(tc, 0f, k);

            yield return null;
        }

        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;
        mainMusic.volume = MAIN_FULL;
    }

    // ================= BOSS =================

        public void EnterBoss()
    {
        if (inBoss) return;

        inBoss = true;

        // Stop any current fades
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        // Start coroutine to play stinger then boss
        fadeCoroutine = StartCoroutine(PlayBossStingerThenLoop());
    }

    IEnumerator PlayBossStingerThenLoop()
{
    // 1️⃣ Play boss stinger immediately
    bossStinger.Play();

    // 2️⃣ Fade out all other layers in parallel
    float fadeTime = fadeDuration;
    float t = 0f;

    float mm = mainMusic.volume;
    float ma = mainAmbience.volume;
    float ta = tribeA.volume;
    float tb = tribeB.volume;
    float tc = tribeC.volume;

    while (t < fadeTime)
    {
        t += Time.unscaledDeltaTime;
        float k = t / fadeTime;

        mainMusic.volume = Mathf.Lerp(mm, 0f, k);
        mainAmbience.volume = Mathf.Lerp(ma, 0f, k);
        tribeA.volume = Mathf.Lerp(ta, 0f, k);
        tribeB.volume = Mathf.Lerp(tb, 0f, k);
        tribeC.volume = Mathf.Lerp(tc, 0f, k);

        yield return null;
    }

    // Snap volumes to 0
    mainMusic.volume = 0f;
    mainAmbience.volume = 0f;
    tribeA.volume = 0f;
    tribeB.volume = 0f;
    tribeC.volume = 0f;

    // 3️⃣ Wait for the stinger to finish
    yield return new WaitWhile(() => bossStinger.isPlaying);

    // 4️⃣ Start boss loop immediately at full volume
    bossMusic.volume = 0.8f;
    bossMusic.Play(); // looping track
}



    public void ExitBoss()
    {
        if (!inBoss) return;

        inBoss = false;
        StartFade(FadeBossToMain());
    }

    IEnumerator FadeToBoss()
    {
        float t = 0f;

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

            mainMusic.volume = Mathf.Lerp(mm, 0f, k);
            mainAmbience.volume = Mathf.Lerp(ma, 0f, k);

            tribeA.volume = Mathf.Lerp(ta, 0f, k);
            tribeB.volume = Mathf.Lerp(tb, 0f, k);
            tribeC.volume = Mathf.Lerp(tc, 0f, k);

            bossMusic.volume = Mathf.Lerp(bm, 1f, k);

            yield return null;
        }

        // 🔒 Boss owns everything
        mainMusic.volume = 0f;
        mainAmbience.volume = 0f;
        tribeA.volume = 0f;
        tribeB.volume = 0f;
        tribeC.volume = 0f;
        bossMusic.volume = 0.8f;
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

    void StartFade(IEnumerator routine, bool isSpawnFade = false)
    {
        if (spawnFadeInProgress && !isSpawnFade)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(routine);
    }
}

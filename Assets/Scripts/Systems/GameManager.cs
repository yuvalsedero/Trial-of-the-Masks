using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public YouDiedUI youDiedUI;
    public float deathScreenTime = 4f;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
        public void PlayerDied()
{
    // Show YOU DIED UI first
    if (youDiedUI != null)
        youDiedUI.Show();

    // Fade out music
    if (MusicManager.Instance != null)
        MusicManager.Instance.FadeOutAll(4f);

    // Fade screen (optional, after showing UI)
    if (ScreenFade.Instance != null)
        ScreenFade.Instance.FadeOut();

    // Restart after delay
    StartCoroutine(RestartAfterDeath());
}

IEnumerator RestartAfterDeath()
{
    // wait for screen fade + music fade
    yield return new WaitForSeconds(Mathf.Max(ScreenFade.Instance.fadeDuration, 4f));

    UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
}

    IEnumerator PlayerDeathSequence()
    {
        // Show YOU DIED text
        if (youDiedUI != null)
            youDiedUI.Show();

        // Fade screen
        if (ScreenFade.Instance != null)
            ScreenFade.Instance.FadeOut(null);

        yield return new WaitForSeconds(deathScreenTime);

        SceneManager.LoadScene("Menu");
    }
    public void LoadSceneAfterDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void RestartAfterDelay(float delay)
    {
        StartCoroutine(RestartCoroutine(delay));
    }

    IEnumerator RestartCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }


    void OnEnable() 
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable() 
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // This finds the script in the current Game scene automatically
    youDiedUI = FindObjectOfType<YouDiedUI>();
}
}

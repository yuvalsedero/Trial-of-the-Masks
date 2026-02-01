using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public YouDiedUI youDiedUI;
    public float deathScreenTime = 2f;
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
        StartCoroutine(PlayerDeathSequence());
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
}

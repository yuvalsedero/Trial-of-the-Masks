using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameController : MonoBehaviour
{
    public CanvasGroup endImage;
    public float fadeInDuration = 3f;
    public float waitBeforeInput = 5f;
    public FlashingText restartText;
    bool canRestart = false;

    void Start()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // Fade image in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            endImage.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }

        endImage.alpha = 1f;

        // Wait before allowing input
        yield return new WaitForSeconds(waitBeforeInput);

        canRestart = true;

        // 🔔 START FLASHING TEXT
        restartText.StartFlashing();
    }


    void Update()
    {
        if (!canRestart)
            return;

        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            SceneManager.LoadScene("Menu"); // 👈 your main scene
        }
    }






    IEnumerator RestartSequence()
{
    if (MusicManager.Instance != null)
        MusicManager.Instance.FadeOutAll(3f);

    yield return new WaitForSeconds(3f);

    SceneManager.LoadScene("Menu");
}
}

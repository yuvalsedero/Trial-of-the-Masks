using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameController : MonoBehaviour
{
    public CanvasGroup endImage;
    public float fadeInDuration = 1.5f;
    public float waitBeforeInput = 5f;

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
    }

    void Update()
    {
        if (!canRestart)
            return;

        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            SceneManager.LoadScene("GameScene"); // 👈 your main scene
        }
    }
}

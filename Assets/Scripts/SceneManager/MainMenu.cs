using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public string mainSceneName = "MainScene";
    bool started = false;

    void Update()
    {
        if (started)
            return;

        // Any key, mouse button, or gamepad
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            started = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainSceneName);
        }
    }

}

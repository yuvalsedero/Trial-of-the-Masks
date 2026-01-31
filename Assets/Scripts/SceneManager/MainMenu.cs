using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string MainScene = "MainScene"; // the name of your gameplay scene

    public void OnBeginButton()
    {
        SceneManager.LoadScene(MainScene);
    }
}

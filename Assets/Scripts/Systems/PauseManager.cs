using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public float volumeLowerAmount = 10f;

    [Header("Player")]
    public PlayerMovement playerMovement;
    public PlayerShoot playerShoot;

    private bool isPaused = false;
    private float originalVolume = 0f;
    private bool originalVolumeCaptured = false;

    private bool canUnpauseByInput = false;

    private const string MASTER_VOLUME_PARAM = "Volume";

    void Start()
    {
        pauseMenuPanel.SetActive(false);


        if (audioMixer != null)
        {
            if (audioMixer.GetFloat(MASTER_VOLUME_PARAM, out originalVolume))
            {
                originalVolumeCaptured = true;
            }
            else
            {
                Debug.LogWarning($"Master volume parameter '{MASTER_VOLUME_PARAM}' not found!");
            }
        }
    }

    void Update()
    {
        // ESC always toggles pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            return;
        }

        // ANY key or mouse resumes (when paused)
        if (isPaused && canUnpauseByInput && Input.anyKeyDown)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);

        if (audioMixer != null && originalVolumeCaptured)
        {
            audioMixer.SetFloat(
                MASTER_VOLUME_PARAM,
                isPaused ? originalVolume - volumeLowerAmount : originalVolume
            );
        }

        Time.timeScale = isPaused ? 0f : 1f;

        if (playerMovement != null)
            playerMovement.canMove = !isPaused;

        if (playerShoot != null)
            playerShoot.canShoot = !isPaused;

        // Prevent instantly unpausing from the same click
        if (isPaused)
            StartCoroutine(EnableAnyKeyResumeNextFrame());
    }

    private System.Collections.IEnumerator EnableAnyKeyResumeNextFrame()
    {
        canUnpauseByInput = false;
        yield return null; // wait one frame
        canUnpauseByInput = true;
    }
}

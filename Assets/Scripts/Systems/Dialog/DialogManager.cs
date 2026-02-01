using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    DialogVoiceSet currentVoice;
    public GameObject dialogCanvas;
    public TextMeshProUGUI dialogText;
    public float typingSpeed = 0.03f;
    bool blockInput = false;

    Queue<string> lines = new Queue<string>();
    System.Action onComplete;

    bool isOpen = false;
    bool isTyping = false;

    string currentLine;
    Coroutine typingCoroutine;

    public AudioSource voiceSource;
    public AudioClip[] lineSounds;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isOpen)
            return;

        if (!blockInput && Input.GetKeyDown(KeyCode.E))
            ShowNextLine();

        if (Input.GetKeyDown(KeyCode.Q))
            CloseDialog();
    }
    public void SetVoice(DialogVoiceSet voiceSet)
    {
        currentVoice = voiceSet;

        if (currentVoice != null)
            lineSounds = currentVoice.clips;
    }
    IEnumerator ShowFirstLineNextFrame()
    {
        yield return null; // wait ONE frame
        ShowNextLine();
    }
    IEnumerator UnblockInputNextFrame()
    {
        yield return null; // wait one frame
        blockInput = false;
    }
    void StopVoice()
    {
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();
    }
    void PlayVoice()
    {

        if (voiceSource == null || lineSounds == null || lineSounds.Length == 0)
            return;

        AudioClip clip = lineSounds[Random.Range(0, lineSounds.Length)];
        voiceSource.clip = clip;
        voiceSource.Play();
    }
    public void OpenDialog(List<string> dialogLines, System.Action onFinish)
    {
        if (isOpen)
            return;

        // 🔴 HARD RESET DIALOG STATE
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StopVoice();
        isTyping = false;
        currentLine = "";
        dialogText.text = "";

        lines.Clear();
        foreach (var l in dialogLines)
            lines.Enqueue(l);

        onComplete = onFinish;

        dialogCanvas.SetActive(true);
        Time.timeScale = 0f;

        isOpen = true;
        blockInput = true;
        StartCoroutine(UnblockInputNextFrame());
        StartCoroutine(ShowFirstLineNextFrame());
    }


    // 🔑 THIS IS THE KEY ADDITION
    public void AppendLines(List<string> moreLines, System.Action onFinish = null)
    {
        foreach (var l in moreLines)
            lines.Enqueue(l);

        if (onFinish != null)
            onComplete = onFinish;
    }

    void ShowNextLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = currentLine;
            StopVoice(); // 🔇 stop talking immediately
            isTyping = false;
            return;
        }
        if (lines.Count == 0)
        {
            CloseDialog();
            return;
        }

        currentLine = lines.Dequeue();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";

        PlayVoice(); // 🔊 START GIBBERISH

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        StopVoice(); // 🔇 STOP GIBBERISH
        isTyping = false;
    }

    void CloseDialog()
    {
        StopVoice();
        dialogCanvas.SetActive(false);
        Time.timeScale = 1f;

        isOpen = false;
        isTyping = false;

        onComplete?.Invoke();
        onComplete = null;
    }

    public bool IsDialogOpen => isOpen;
}

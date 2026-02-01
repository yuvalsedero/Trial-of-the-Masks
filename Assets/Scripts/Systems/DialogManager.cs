using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialogCanvas;
    public TextMeshProUGUI dialogText;
    public float typingSpeed = 0.03f;

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

        if (Input.GetKeyDown(KeyCode.E))
            ShowNextLine();

        if (Input.GetKeyDown(KeyCode.Q))
            CloseDialog();
    }

    public void OpenDialog(List<string> dialogLines, System.Action onFinish)
    {
        if (isOpen)
            return;

        lines.Clear();
        foreach (var l in dialogLines)
            lines.Enqueue(l);

        onComplete = onFinish;

        dialogCanvas.SetActive(true);
        Time.timeScale = 0f;

        isOpen = true;
        ShowNextLine();
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

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void CloseDialog()
    {
        dialogCanvas.SetActive(false);
        Time.timeScale = 1f;

        isOpen = false;
        isTyping = false;

        onComplete?.Invoke();
        onComplete = null;
    }

    public bool IsDialogOpen => isOpen;
}

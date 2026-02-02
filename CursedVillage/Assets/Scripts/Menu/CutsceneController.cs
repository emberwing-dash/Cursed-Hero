using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{

    [System.Serializable]
    public class CameraShot
    {
        public Camera cam;

        [Tooltip("Ignored if Use Dialogue is true")]
        public float duration = 3f;

        [Header("Dialogue")]
        public bool useDialogue = false;

        [TextArea(2, 5)]
        public string[] dialogueLines;
    }

    [Header("Shots Order")]
    [SerializeField] private List<CameraShot> shots = new List<CameraShot>();

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Typing")]
    [SerializeField] private float typeSpeed = 0.03f;

    [Header("Input")]
    [SerializeField] private KeyCode nextKey = KeyCode.E;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName;


    private int currentShotIndex = -1;
    private Coroutine typingRoutine;
    private bool isTyping;
    private bool waitingForInput;
    private Coroutine cutsceneRoutine;


    private void Start()
    {
        DisableAllCameras();

        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);

        cutsceneRoutine = StartCoroutine(PlayCutscene());
    }


    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < shots.Count; i++)
        {
            currentShotIndex = i;

            CameraShot shot = shots[i];

            ActivateCamera(shot.cam);

            // Dialogue shot
            if (shot.useDialogue && shot.dialogueLines.Length > 0)
            {
                yield return StartCoroutine(PlayDialogue(shot.dialogueLines));
            }
            else
            {
                yield return new WaitForSeconds(shot.duration);
            }
        }

        EndCutscene();
    }


    void DisableAllCameras()
    {
        foreach (var s in shots)
            if (s.cam != null)
                s.cam.gameObject.SetActive(false);
    }

    void ActivateCamera(Camera cam)
    {
        DisableAllCameras();

        if (cam != null)
            cam.gameObject.SetActive(true);
    }


    IEnumerator PlayDialogue(string[] lines)
    {
        dialogueCanvas.SetActive(true);

        for (int i = 0; i < lines.Length; i++)
        {
            yield return StartCoroutine(TypeLine(lines[i]));
            yield return StartCoroutine(WaitForNextKey());
        }

        dialogueCanvas.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        dialogueText.text = "";
        isTyping = true;

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    IEnumerator WaitForNextKey()
    {
        waitingForInput = true;

        while (true)
        {
            if (Input.GetKeyDown(nextKey))
            {
                // If still typing → instantly finish line
                if (isTyping)
                {
                    isTyping = false;
                    yield break;
                }

                break;
            }

            yield return null;
        }

        waitingForInput = false;
    }


    void EndCutscene()
    {
        DisableAllCameras();
        Debug.Log("Cutscene Finished");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }


    public void SkipCutscene()
    {
        // Stop all running coroutines
        if (cutsceneRoutine != null)
            StopCoroutine(cutsceneRoutine);

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        dialogueCanvas.SetActive(false);
        EndCutscene();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerPortrait;
    [TextArea(2, 5)]
    public string text;
}

public class DialogueTyper : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject dialogueUI;

    [Header("Typing Settings")]
    [SerializeField] private float typeSpeed = 0.04f;

    private DialogueLine[] currentDialogueLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;

    private ThirdPersonController playerController;

    // Trigger reference (to optionally destroy)
    private CharacterDialogueTrigger currentTrigger;

    [Header("Animator (optional)")]
    [SerializeField] private Animator characterAnimator;

    void Start()
    {
        dialogueUI.SetActive(false);
        playerController = FindObjectOfType<ThirdPersonController>();
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentDialogueLines[currentLineIndex].text;
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < currentDialogueLines.Length)
                {
                    StartCoroutine(TypeLine(currentDialogueLines[currentLineIndex]));
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    public void StartDialogue(DialogueLine[] lines, CharacterDialogueTrigger trigger)
    {
        if (lines == null || lines.Length == 0) return;

        currentDialogueLines = lines;
        currentLineIndex = 0;
        dialogueActive = true;
        dialogueUI.SetActive(true);
        currentTrigger = trigger;

        if (playerController != null)
            playerController.enabled = false;

        if (characterAnimator != null)
            characterAnimator.SetBool("isTalking", true);

        StartCoroutine(TypeLine(currentDialogueLines[currentLineIndex]));
    }

    public void SetAnimator(Animator anim)
    {
        characterAnimator = anim;
    }

    void EndDialogue()
    {
        dialogueActive = false;
        dialogueUI.SetActive(false);
        currentDialogueLines = null;
        currentLineIndex = 0;

        if (playerController != null)
            playerController.enabled = true;

        if (characterAnimator != null)
            characterAnimator.SetBool("isTalking", false);

        // Let trigger notify manager
        if (currentTrigger != null)
        {
            currentTrigger.NotifyDialogueCompleted();
            currentTrigger = null;
        }
    }


    IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;

        if (nameText != null) nameText.text = line.speakerName;
        if (portraitImage != null) portraitImage.sprite = line.speakerPortrait;

        dialogueText.text = "";
        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }
}

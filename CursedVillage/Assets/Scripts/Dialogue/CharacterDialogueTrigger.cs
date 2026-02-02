using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueSet
{
    public DialogueLine[] lines;
}

public class CharacterDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Sets")]
    [SerializeField] private List<DialogueSet> dialogueSets;

    [Header("Animator (optional)")]
    [SerializeField] private Animator characterAnimator;

    [Header("Trigger Options")]
    [Tooltip("Destroy this trigger after dialogue ends?")]
    public bool destroyAfterDialogue = true;

    // Event for sequence manager
    public event Action<CharacterDialogueTrigger> OnDialogueCompleted;

    private DialogueTyper dialogueSystem;
    private bool playerInTrigger = false;
    private int currentDialogueSetIndex = 0;

    void Start()
    {
        dialogueSystem = FindObjectOfType<DialogueTyper>();
        if (dialogueSystem != null && characterAnimator != null)
            dialogueSystem.SetAnimator(characterAnimator);
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueSystem != null && currentDialogueSetIndex < dialogueSets.Count)
            {
                dialogueSystem.StartDialogue(dialogueSets[currentDialogueSetIndex].lines, this);
                currentDialogueSetIndex++;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = false;
    }

    public void NotifyDialogueCompleted()
    {
        // Fire event so manager can activate next trigger
        OnDialogueCompleted?.Invoke(this);

        // Then destroy this trigger object if flagged
        if (destroyAfterDialogue)
            Destroy(gameObject);
    }
}

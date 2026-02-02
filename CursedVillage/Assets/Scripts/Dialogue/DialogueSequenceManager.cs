using System.Collections.Generic;
using UnityEngine;

public class DialogueSequenceManager : MonoBehaviour
{
    [Header("Dialogue Triggers in Sequence")]
    [SerializeField] private List<CharacterDialogueTrigger> dialogueTriggers;

    private int currentIndex = 0;

    void Start()
    {
        // Disable all except first
        for (int i = 0; i < dialogueTriggers.Count; i++)
        {
            if (dialogueTriggers[i] != null)
                dialogueTriggers[i].gameObject.SetActive(i == 0);

            // Subscribe to OnDialogueCompleted event
            if (dialogueTriggers[i] != null)
                dialogueTriggers[i].OnDialogueCompleted += HandleDialogueCompleted;
        }
    }

    private void HandleDialogueCompleted(CharacterDialogueTrigger finishedTrigger)
    {
        currentIndex++;

        if (currentIndex < dialogueTriggers.Count)
        {
            var nextTrigger = dialogueTriggers[currentIndex];
            if (nextTrigger != null)
                nextTrigger.gameObject.SetActive(true);
        }
    }
}

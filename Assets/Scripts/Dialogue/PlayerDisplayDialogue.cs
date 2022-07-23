using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDisplayDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public void StartConversation(DialogueTrigger trigger, Dialogue message)
    {
        ResetUI();
        StopAllCoroutines();
        StartCoroutine(DialogueSequence(trigger, message));
    }

    private IEnumerator DialogueSequence(DialogueTrigger trigger, Dialogue dialogue)
    {
        trigger.Talking = true;
        dialogueBox.SetActive(true);

        nameText.text = dialogue.Name;

        for (int i = 0; i < dialogue.Monologues.Count; i++)
        {
            IDialogueSection message = dialogue.Monologues[i];
            dialogueText.text = message.ReceievePrompt();

            while (!Input.GetKeyDown(KeyCode.E)) yield return null;

            yield return null;
        }

        trigger.Talking = false;
        ResetUI();
    }

    private void ResetUI()
    {
        dialogueBox.SetActive(false);
        nameText.text = "";
        dialogueText.text = "";
    }
}

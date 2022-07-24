using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDisplayDialogue : MonoBehaviour
{
    [SerializeField] private ClubEquipController dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    private Dialogue message = null;
    private DialogueTrigger trigger = null;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        player.PlayerInput.OnDialogueInput += UpdateDialogue;
    }

    int i = 0;
    private void UpdateDialogue(bool dialogueSkip)
    {
        if (message == null || trigger == null)
        {
            ResetUI();
            return;
        }

        trigger.Talking = true;
        nameText.text = message.Name;

        while (i <= message.Monologues.Count)
        {
            if (!dialogueSkip) return;
            else if (i == message.Monologues.Count) break;

            IDialogueSection section = message.Monologues[i];
            dialogueText.text = section.ReceievePrompt();
            section.Accept();
            i++;
            return;
        }

        dialogueBox.HideUI();
        trigger.Talking = false;
        message = null;
        trigger = null;
    }

    public void StartConversation(DialogueTrigger trigger, Dialogue message)
    {
        ResetUI();

        this.message = message;
        this.trigger = trigger;

        dialogueBox.HideUI(false);
    }

    private void ResetUI()
    {
        nameText.text = "";
        dialogueText.text = "";
        i = 0;
    }
}

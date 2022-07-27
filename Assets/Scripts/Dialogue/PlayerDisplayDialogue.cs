using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDisplayDialogue : MonoBehaviour
{
    [SerializeField] private AudioClip dialogueCharacter;
    [SerializeField] private UIAnimationController dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float timeBetweenCharacters;
    private Dialogue message = null;
    private DialogueTrigger trigger = null;
    private bool finishedTypeWriting = false;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake() => player.PlayerInput.OnDialogueInput += UpdateDialogue;

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
            if (!dialogueSkip && i > 0) return;

            if (!finishedTypeWriting && dialogueSkip)
            { 
                StopAllCoroutines();
                dialogueText.text = message.Monologues[i - 1].ReceievePrompt();
                finishedTypeWriting = true;
                return;
            }

            if (i == message.Monologues.Count) break;

            IDialogueSection section = message.Monologues[i];

            dialogueBox.SetPositionOffsetRecoil(Vector3.down * 20f);
            section.DialogueAction?.Invoke();

            StopAllCoroutines();
            StartCoroutine(TypeWriteMonologue(section.ReceievePrompt()));

            dialogueBox.UIShake.ShakeOnce(new PerlinShake(ShakeData.Create(section.Intensity, 7f, 1f, 10f)));
            i++;
            return;
        }

        dialogueBox.HideUI();
        trigger.Talking = false;
        message = null;
        trigger = null;
    }

    public IEnumerator TypeWriteMonologue(string sentence)
    {
        finishedTypeWriting = false;

        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            AudioManager.Instance.PlayOnce(dialogueCharacter, transform.position);

            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        finishedTypeWriting = true;
    }

    public void StartConversation(DialogueTrigger trigger, Dialogue message)
    {
        if (this.trigger != null) this.trigger.Talking = false;

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

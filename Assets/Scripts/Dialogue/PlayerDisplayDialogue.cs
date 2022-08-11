using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDisplayDialogue : MonoBehaviour
{
    [Header("Main Dialogue")]
    [SerializeField] private AudioClip dialogueCharacter;
    [SerializeField] private UIAnimationController dialogueBox;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float timeBetweenCharacters;

    [Header("Options Dialogue")]
    [SerializeField] private GameObject optionUI;
    [SerializeField] private List<TextMeshProUGUI> optionsTexts = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> optionsTextsKeyBinds = new List<TextMeshProUGUI>();
    [SerializeField] private GameObject skipHintText;

    private Dialogue message = null;

    private List<Monologue> monologueQueue = new List<Monologue>();
    private Options currentOptions = null;

    private DialogueTrigger trigger = null;
    private bool finishedTypeWriting = false;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        player.PlayerInput.OnDialogueInput += UpdateDialogue;
        player.PlayerInput.OnDialogueOptionsInput += UpdateOptions;
    }

    int i = 0;
    private void UpdateDialogue(bool dialogueSkip)
    {
        //Handle Input 
        if (!dialogueSkip) return;

        //If there is no message or speaker return
        if (message == null || trigger == null)
        {
            ResetUI();
            return;
        }

        if (monologueQueue.Count == 0)
        {
            ExitDialogue();
            return;
        }

        trigger.Talking = true;

        //Dont skip using E if there is a question
        if (currentOptions != null) return;

        //If the entire message hasnt been written yet, skip to its finished state
        if (!finishedTypeWriting)
        {
            StopAllCoroutines();
            dialogueText.text = monologueQueue[i - 1].OpenPrompt;
            finishedTypeWriting = true;
            return;
        }

        //Dialogue is Finished
        if (i == monologueQueue.Count)
        {
            ExitDialogue();
            return;
        }

        Display();
        return;

        void ExitDialogue()
        {
            dialogueBox.HideUI();
            trigger.Talking = false;
            message = null;
            trigger = null;
            currentOptions = null;
        }
    }

    private void Display()
    {
        if (i > monologueQueue.Count - 1) return;

        Monologue section = monologueQueue[i];
        if (section == null)
        {
            i++;
            return;
        }

        currentOptions = section.Branch;

        dialogueBox.SetPositionOffsetRecoil(Vector3.down * 20f);
        section.DialogueAction?.Invoke();

        StopAllCoroutines();
        StartCoroutine(TypeWriteMonologue(section.OpenPrompt));

        dialogueBox.UIShake.ShakeOnce(new PerlinShake(ShakeData.Create(section.Intensity, 7f, 1f, 10f)));
        skipHintText.SetActive(currentOptions == null);
        i++;
    }

    private void UpdateOptions(int index)
    {
        if (currentOptions == null)
        {
            ResetOptionsUI();
            return;
        }

        //UI Update
        optionUI.SetActive(true);
        for (int i = 0; i < currentOptions.OptionTexts.Length; i++)
        {
            if (i >= optionsTexts.Count) continue;
            if (i >= optionsTextsKeyBinds.Count) continue;

            optionsTexts[i].text = currentOptions.OptionTexts[i];
            optionsTextsKeyBinds[i].text = player.PlayerInput.DialogueOptionsKeys[i].ToString();
        }

        //If an option hasnt been picked yet do nothing
        if (index == -1) return;

        //Make sure we select a valid dialogue branch based on our option
        if (currentOptions.DialogueContinuations.Length == 0 || index >= currentOptions.DialogueContinuations.Length)
        {
            //If there isnt a valid followup branch just skip to the next monologue
            currentOptions = null;
            finishedTypeWriting = true;
            UpdateDialogue(true);
            return;
        }

        Dialogue addedDialogue = currentOptions.DialogueContinuations[index];

        //Add the follow up monologues of that option
        for (int i = addedDialogue.Monologues.Count - 1; i > -1; i--)
        {
            Monologue addedMonologue = addedDialogue.Monologues[i];

            if (addedMonologue == null) continue;
            monologueQueue.Insert(this.i, addedMonologue);
        }

        //Skip to the next monologue
        currentOptions = null;
        finishedTypeWriting = true;
        UpdateDialogue(true);
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
        if (message.Monologues.Count == 0) return;

        if (this.trigger != null) this.trigger.Talking = false;

        monologueQueue.Clear();

        ResetUI();
        ResetOptionsUI();

        this.message = message;
        this.trigger = trigger;

        dialogueBox.HideUI(false);
        monologueQueue = new List<Monologue>(message.Monologues);
        Display();

        nameText.text = trigger.gameObject.name;
    }

    private void ResetUI()
    {
        nameText.text = "";
        dialogueText.text = "";
        i = 0;      
    }

    private void ResetOptionsUI()
    {
        optionUI.SetActive(false);
        foreach (TextMeshProUGUI text in optionsTexts) text.text = "";
    }
}

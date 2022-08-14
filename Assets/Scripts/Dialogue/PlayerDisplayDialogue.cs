using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private TextMeshProUGUI[] optionsTexts = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] optionsTextsKeyBinds = new TextMeshProUGUI[2];
    [SerializeField] private UIColorController[] selectEffects = new UIColorController[2];
    [SerializeField] private GameObject skipHintText;

    [Header("Emotions")]
    [SerializeField] private Image authorEmotionImage;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite sadSprite;
    [SerializeField] private Sprite scaredSprite;
    [SerializeField] private Sprite angrySprite;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite deathSprite;
    [SerializeField] private Sprite confusedSprite;
    [SerializeField] private Sprite lustSprite;
    [SerializeField] private Sprite screamSprite;
    [SerializeField] private Sprite thinkingSprite;
    [SerializeField] private Sprite laughingSprite;

    private Dialogue currentMessage = null;
    private DialogueAction currentDialogueAction = null;

    private List<Monologue> monologueQueue = new List<Monologue>();
    private Options currentOptions = null;

    private DialogueTrigger currentTrigger = null;
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
        if (currentMessage == null || currentTrigger == null)
        {
            ResetUI();
            return;
        }

        if (monologueQueue.Count == 0)
        {
            ExitDialogue();
            return;
        }

        //If the entire message hasnt been written yet, skip to its finished state
        if (!finishedTypeWriting)
        {
            StopAllCoroutines();
            dialogueText.text = monologueQueue[i - 1].OpenPrompt;
            finishedTypeWriting = true;
            return;
        }

        //Dont skip using E if there is a question
        if (currentOptions != null) return;

        //Dialogue is Finished
        if (i == monologueQueue.Count)
        {
            ExitDialogue();
            FireDialogueEvent(DialougeActionTime.End);
            return;
        }

        FireDialogueEvent(DialougeActionTime.End);

        Display();
        return;       
    }

    private void Display()
    {
        if (i > monologueQueue.Count - 1) return;

        //If there is no monologue seciton here, just skip it
        Monologue section = monologueQueue[i];
        if (section == null)
        {
            i++;
            return;
        }

        //Save the current option branch we are on
        currentOptions = section.Branch;
        skipHintText.SetActive(currentOptions == null);

        //UI bob effect
        dialogueBox.SetPositionOffsetRecoil(Vector3.down * 20f);
        dialogueBox.UIShake.ShakeOnce(new PerlinShake(ShakeData.Create(section.Intensity, 7f, 1f, 10f)));

        //Get action from monologue
        currentDialogueAction = currentMessage.FindAction(section);
        FireDialogueEvent(DialougeActionTime.Start);

        //Do the typewriter effect for dialogue
        StopAllCoroutines();
        StartCoroutine(TypeWriteMonologue(section.OpenPrompt));

        //Set appropiate profile for speaker
        if (currentMessage.Author != null)
        {
            nameText.text = currentMessage.Author.AuthorName;
            authorEmotionImage.color = Color.white;
            authorEmotionImage.sprite = SelectEmotion(section.AuthorEmotion);
        }

        //Move forward
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
            if (i >= optionsTexts.Length) continue;
            if (i >= optionsTextsKeyBinds.Length) continue;

            optionsTexts[i].text = currentOptions.OptionTexts[i];
            optionsTextsKeyBinds[i].text = player.PlayerInput.DialogueOptionsKeys[i].ToString();
        }

        //If an option hasnt been picked yet do nothing
        if (index == -1) return;

        //Select effect
        selectEffects[index].SnapOpacity(80f);

        //Make sure we select a valid dialogue branch based on our option
        if (currentOptions.DialogueContinuations.Length == 0)
        {
            //If there isnt a valid followup branch just skip to the next monologue
            currentOptions = null;
            finishedTypeWriting = true;
            UpdateDialogue(true);
            return;
        }

        //Make sure we can only select a valid branch if there is one
        if (index >= currentOptions.DialogueContinuations.Length) return;

        Branch addedDialogue = currentOptions.DialogueContinuations[index];
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
        //If there is no message or speaker return
        if (trigger == null || message == null)
        {
            ResetUI();
            return;
        }

        if (message.Monologues.Count == 0) return;
        if (currentTrigger != null) currentTrigger.Talking = false;

        monologueQueue.Clear();
        ResetUI();
        ResetOptionsUI();

        currentMessage = message;
        currentTrigger = trigger;
        currentTrigger.Talking = true;

        dialogueBox.HideUI(false);
        monologueQueue = new List<Monologue>(currentMessage.Monologues);
        Display();
    }

    private void ResetUI()
    {
        nameText.text = "";
        dialogueText.text = "";
        authorEmotionImage.color = Color.clear;
        i = 0;
    }

    private void ResetOptionsUI()
    {
        optionUI.SetActive(false);
        foreach (TextMeshProUGUI text in optionsTexts) text.text = "";
    }

    private Sprite SelectEmotion(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Happy: return happySprite;
            case Emotion.Sad: return sadSprite;
            case Emotion.Scared: return scaredSprite;
            case Emotion.Normal: return normalSprite;
            case Emotion.Angry: return angrySprite;
            case Emotion.Death: return deathSprite;
            case Emotion.Confused: return confusedSprite;
            case Emotion.Lust: return lustSprite;
            case Emotion.Thinking: return thinkingSprite;
            case Emotion.Scream: return screamSprite;
            case Emotion.Laughing: return laughingSprite;
            default: return normalSprite;
        }
    }

    void ExitDialogue()
    {
        dialogueBox.HideUI();
        currentTrigger.Talking = false;
        currentMessage = null;
        currentTrigger = null;
        currentOptions = null;
    }

    void FireDialogueEvent(DialougeActionTime timeToFire)
    {
        if (currentDialogueAction != null && currentDialogueAction.Time == timeToFire)
            currentDialogueAction.MonologueEvent?.Invoke();
    }
}

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

    private Dialogue message = null;
    private Options currentOptions;
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
        if (message == null || trigger == null)
        {
            ResetUI();
            return;
        }

        trigger.Talking = true;
        nameText.text = message.Name;

        while (i <= message.Monologues.Count)
        {
            //Handle Input 
            {
                if (!dialogueSkip && i > 0) return;

                if (!finishedTypeWriting && dialogueSkip)
                {
                    StopAllCoroutines();
                    dialogueText.text = message.Monologues[i - 1].OpenPrompt;
                    finishedTypeWriting = true;
                    return;
                }

                if (i == message.Monologues.Count) break;
            }

            Monologue section = message.Monologues[i];
            currentOptions = section.Branch;

            dialogueBox.SetPositionOffsetRecoil(Vector3.down * 20f);
            section.DialogueAction?.Invoke();

            StopAllCoroutines();
            StartCoroutine(TypeWriteMonologue(section.OpenPrompt));

            dialogueBox.UIShake.ShakeOnce(new PerlinShake(ShakeData.Create(section.Intensity, 7f, 1f, 10f)));
            i++;
            return;
        }

        dialogueBox.HideUI();
        trigger.Talking = false;
        message = null;
        trigger = null;
        currentOptions = null;
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
        for (int i = 0; i < currentOptions.OptionTexts.Length; i++) optionsTexts[i].text = currentOptions.OptionTexts[i];
    
        
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

    private void ResetOptionsUI()
    {
        optionUI.SetActive(false);
        foreach (TextMeshProUGUI text in optionsTexts) text.text = "";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbbieAndCarbon : MonoBehaviour
{
    [SerializeField] private Dialogue abbieDialogue;
    [SerializeField] private Dialogue carbonDialogue;
    [SerializeField] private DialogueTrigger carbonTrigger;
    [SerializeField] private DialogueTrigger abbieTrigger;

    public void AlertCarbon()
    {
        abbieTrigger.Player.DialogueHandler.StartConversation(carbonTrigger, carbonDialogue);
    }

    public void AlertAbbie()
    {
        abbieTrigger.Player.DialogueHandler.StartConversation(abbieTrigger, abbieDialogue);
    }
}

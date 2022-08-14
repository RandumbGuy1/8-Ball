using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConversation : MonoBehaviour
{
    [SerializeField] private DialogueTrigger convoStarter;
    [SerializeField] private Conversator[] convesators;

    public void AlertTrigger(int index)
    {
        convoStarter.Player.DialogueHandler.StartConversation(convesators[index].Trigger, convesators[index].NextDialogue());
    }
}

[System.Serializable]
public class Conversator
{
    [SerializeField] private DialogueTrigger trigger;
    [SerializeField] private Dialogue[] dialouges;
    private int index = -1;

    public Dialogue NextDialogue()
    {
        index++;

        if (index >= dialouges.Length) index = 0;
        if (index < 0) return null;

        return dialouges[index];
    }

    public DialogueTrigger Trigger => trigger;
}

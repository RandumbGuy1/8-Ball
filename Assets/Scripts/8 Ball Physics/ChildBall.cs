using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildBall : MonoBehaviour, IEightBall
{
    [SerializeField] private Dialogue hurtDialogue;
    [SerializeField] private Dialogue trauma;
    [SerializeField] private EightBallFather father;
    [SerializeField] private DialogueTrigger trigger;

    public void ClubBall(PlayerRef player)
    {
        player.DialogueHandler.StartConversation(trigger, hurtDialogue);
        father.AlertFather(player);

        trigger.SetNewDialogue(trauma);
    }
}

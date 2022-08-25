using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private ParticleSystem notification;
    [SerializeField] private Dialogue dialogue;
    public bool Talking { get; set; } = false;
    public PlayerRef Player { get; set; }

    public GameObject GameObject => gameObject;

    public string GetDescription(PlayerRef player)
    {
        return Talking ? null : "Talk to NPC";
    }

    public void OnEndHover(PlayerRef player)
    {

    }

    public void OnInteract(PlayerRef player)
    {
        if (player == null) return;

        player.DialogueHandler.StartConversation(this, dialogue);
        if (notification != null) notification.Stop();
    }

    public void OnStartHover(PlayerRef player)
    {

    }

    public void SetNewDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        if (notification != null) notification.Play();
    }
}

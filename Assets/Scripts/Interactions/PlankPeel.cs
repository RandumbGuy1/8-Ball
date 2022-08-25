using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankPeel : MonoBehaviour, IInteractable
{
    [Header("Optional Dialogue")]
    [SerializeField] private Dialogue peelDialogue;
    [SerializeField] private DialogueTrigger trigger;

    [Header("References")]
    [SerializeField] private Rigidbody plankRb;
    [SerializeField] private AudioClip peelClip;

    public PlayerRef Player { get; set; }
    public GameObject GameObject => gameObject;

    private bool peeled = false;

    public string GetDescription(PlayerRef player)
    {
        return peeled ? null : "Remove Plank";
    }

    public void OnEndHover(PlayerRef player)
    {
        
    }

    public void OnInteract(PlayerRef player)
    {
        player.DialogueHandler.StartConversation(trigger, peelDialogue);

        AudioManager.Instance.PlayOnce(peelClip, transform.position);
        plankRb.isKinematic = false;
        peeled = true;
    }

    public void OnStartHover(PlayerRef player)
    {
        
    }
}

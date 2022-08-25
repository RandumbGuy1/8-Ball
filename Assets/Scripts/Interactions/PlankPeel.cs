using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankPeel : MonoBehaviour, IInteractable
{
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
        AudioManager.Instance.PlayOnce(peelClip, transform.position);
        plankRb.isKinematic = false;
        peeled = true;
    }

    public void OnStartHover(PlayerRef player)
    {
        
    }
}

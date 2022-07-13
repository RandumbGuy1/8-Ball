using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public GameObject GameObject => gameObject;
    public bool PickedUp { get; set; }

    public string GetDescription(PlayerRef player) => PickedUp ? null : "Pickup Item";

    public void OnEndHover(PlayerRef player)
    {
        
    }

    public void OnInteract(PlayerRef player) => player.ClubHolder.AddClub(gameObject);

    public void OnStartHover(PlayerRef player)
    {
        
    }
}

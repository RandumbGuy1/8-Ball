using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] Collider itemCol;
    public GameObject GameObject => gameObject;

    public string GetDescription(PlayerRef player)
    {
        return itemCol.isTrigger ? null : "Pickup Item";
    }

    public void OnEndHover(PlayerRef player)
    {
        
    }

    public void OnInteract(PlayerRef player)
    {
        itemCol.isTrigger = true;
        player.ClubHolder.AddClub(gameObject);
    }

    public void OnStartHover(PlayerRef player)
    {
        
    }
}

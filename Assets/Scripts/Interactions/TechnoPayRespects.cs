using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnoPayRespects : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject technoQuotes;

    public GameObject GameObject => gameObject;
    private bool used = false;

    public string GetDescription(PlayerRef player)
    {
        if (used) return null;
        return "Pay Respects";
    }

    public void OnEndHover(PlayerRef player)
    {
        
    }

    public void OnInteract(PlayerRef player)
    {
        used = true;
        technoQuotes.SetActive(true);
    }

    public void OnStartHover(PlayerRef player)
    {
        
    }
}

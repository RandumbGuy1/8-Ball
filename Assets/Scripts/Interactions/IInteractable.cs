using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    PlayerRef Player { get; set; }
    GameObject GameObject { get; }
    string GetDescription(PlayerRef player);

    void OnInteract(PlayerRef player);
    void OnStartHover(PlayerRef player);
    void OnEndHover(PlayerRef player);
}

using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip[] pickupClips = new AudioClip[0];
    public PlayerRef Player { get; set; }

    public GameObject GameObject => gameObject;
    public bool PickedUp { get; set; }

    public string GetDescription(PlayerRef player) => PickedUp ? null : "Pickup Item";

    public void OnEndHover(PlayerRef player)
    {
        
    }

    public void OnInteract(PlayerRef player)
    {
        player.ClubHolder.AddClub(gameObject);
        AudioManager.Instance.PlayOnce(pickupClips, transform.position);
    }

    public void OnStartHover(PlayerRef player)
    {
        
    }
}

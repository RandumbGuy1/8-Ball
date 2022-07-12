using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicClub : MonoBehaviour, IClub, IItem
{
    [SerializeField] private ClubStats stats = new ClubStats();
    [SerializeField] private ItemHoldSettings holdSettings = new ItemHoldSettings();
    [SerializeField] private ItemArtSettings clubSpriteSettings = new ItemArtSettings();
    [SerializeField] private List<ItemArtSettings> clubAbilitySpriteSettings = new List<ItemArtSettings>();

    public ClubStats Stats => stats;
    public ItemHoldSettings HoldSettings => holdSettings;
    public ItemArtSettings SpriteSettings => clubSpriteSettings;
    public List<ItemArtSettings> AbilitySpriteSettings => clubAbilitySpriteSettings;

    public void OnDrop(PlayerRef player)
    {
        
    }

    public void OnPickup(PlayerRef player)
    {
        
    }

    public void ThrustBalls(PlayerRef player)
    {
        print(player + ": Thrusted my balls :(");
    }

    public void UseClubAbility(PlayerRef player)
    {
        print(player + ": Used club ability on my balls :(");
    }

    public void UseMovementAbility(PlayerRef player)
    {
        print(player + ": Used movement ability on my balls :(");
    }
}

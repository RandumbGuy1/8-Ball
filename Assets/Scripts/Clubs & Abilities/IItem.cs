using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem 
{
    ItemHoldSettings HoldSettings { get; }
    ItemArtSettings SpriteSettings { get; }
    List<ItemArtSettings> AbilitySpriteSettings { get; }

    void OnPickup(PlayerRef player);
    void OnDrop(PlayerRef player);
}

[System.Serializable]
public struct ItemArtSettings
{
    [SerializeField] private string itemText;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Vector3 scale;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Vector3 position;
    [SerializeField] private bool enabled;

    public string ItemText => itemText;
    public Sprite ItemSprite => itemSprite;
    public Vector3 Scale => scale;
    public Vector3 Rotation => rotation;
    public Vector3 Position => position;
    public bool Enabled => enabled;
}

[System.Serializable]
public struct ItemHoldSettings
{
    
}

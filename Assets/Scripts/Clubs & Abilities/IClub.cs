using UnityEngine;
using System.Collections.Generic;

public interface IClub
{
    ItemArtSettings ClubSpriteSettings { get; }
    List<ItemArtSettings> ClubAbilitySpriteSettings { get; }

    ClubStats Stats { get; }

    void ThrustBalls(PlayerRef player);
    void UseMovementAbility(PlayerRef player);
    void UseClubAbility(PlayerRef player);
}

[System.Serializable]
public struct ClubStats
{
    [SerializeField] private float clubLength;
    [SerializeField] private float clubChargeTime;
    [SerializeField] private float maxClubCharge;
    [SerializeField] private float upwardClamp;
    [SerializeField] private float clubPower;
    [SerializeField] private int predictionCount;

    public float ClubLength => clubLength;
    public float ClubChargeTime => clubChargeTime;
    public float MaxClubCharge => maxClubCharge;
    public float UpwardClamp => upwardClamp;
    public float ClubPower => clubPower;
    public int PredictionCount => predictionCount;
}

[System.Serializable]
public struct ItemArtSettings
{
    [SerializeField] private string itemText;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private Vector3 scale;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Vector3 position;

    public string ItemText => itemText;
    public Sprite ItemSprite => itemSprite;
    public Vector3 Scale => scale;
    public Vector3 Rotation => rotation;
    public Vector3 Position => position;
}

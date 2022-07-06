using UnityEngine;

public interface IClub
{
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

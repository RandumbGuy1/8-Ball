using UnityEngine;

[System.Serializable]
public class SubmergeeData
{
    public PlayerRef Player { get; private set; }
    public Collider Col { get; private set; }
    public Rigidbody Rb { get; private set; }
    public ParticleSystem Ripples { get; private set; }

    public SubmergeeData(PlayerRef player, Collider col, Rigidbody rb, ParticleSystem ripples)
    {
        Player = player;
        Col = col;
        Rb = rb;
        Ripples = ripples;
    }
}

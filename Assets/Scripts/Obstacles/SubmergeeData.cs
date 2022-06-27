using UnityEngine;

[System.Serializable]
public class SubmergeeData
{
    public Collider Col { get; private set; }
    public Rigidbody Rb { get; private set; }
    public ParticleSystem Ripples { get; private set; }

    public SubmergeeData(Collider col, Rigidbody rb, ParticleSystem ripples)
    {
        Col = col;
        Rb = rb;
        Ripples = ripples;
    }
}

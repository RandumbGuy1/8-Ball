using UnityEngine;

[System.Serializable]
public class MassPoint 
{
    [SerializeField] private Transform anchor;
    [SerializeField] private float mass;
    [SerializeField] private Vector3 position;
    [SerializeField] private bool freezePosition = false;

    public Transform Anchor => anchor;
    public Vector3 Position => position;
    public Vector3 Velocity { get; set; } = Vector3.zero;
    public Vector3 Force { get; set; } = Vector3.zero;
    public float Mass => mass;

    public void PhysicsUpdate()
    {
        if (anchor != null) anchor.localPosition = position;

        if (freezePosition)
        {
            Velocity = Vector3.zero;
            Force = Vector3.zero;
            return;
        }

        Velocity += (Force * Time.fixedDeltaTime) / Mass;
        position += Velocity * Time.fixedDeltaTime;

        Force = Vector3.zero;
    }
}

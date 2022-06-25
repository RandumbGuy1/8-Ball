using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private MassPoint a = new MassPoint();
    [SerializeField] private MassPoint b = new MassPoint();
    public MassPoint A => a;
    public MassPoint B => b;

    [Header("Spring Settings")]
    [SerializeField] private float dampening;
    [SerializeField] private float restLength;
    [SerializeField] private float stiffness;

    void FixedUpdate()
    {
        a.PhysicsUpdate();
        b.PhysicsUpdate();

        float forceSustained = ((b.Position - a.Position).magnitude - restLength) * stiffness;
        float forceDampening = Vector3.Dot((b.Position - a.Position).normalized, b.Velocity - a.Velocity) * dampening;

        float totalForce = forceSustained + forceDampening;

        a.Force = (b.Position - a.Position).normalized * totalForce;
        b.Force = (a.Position - b.Position).normalized * totalForce;
    }
}

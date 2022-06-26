using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;

    public Vector2 Input { get; private set; } = Vector2.zero;

    public bool Jumping { get; private set; } = false;
    public bool Moving { get; private set; } = false;

    public float Magnitude { get; private set; }
    public Vector3 RelativeVel { get; private set; }
    public Vector3 Velocity { get; private set; }

    [Header("Hover Settings")]
    [SerializeField] private LayerMask environment;
    [SerializeField] private float rideRayExtension;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    public bool InWater { get; set; }
    public bool Grounded { get; private set; }

    [Header("Standing Settings")]
    [SerializeField] private float uprightSpringStrength;
    [SerializeField] private float uprightSpringStrengthDamper;

    [Header("Friction Settings")]
    [SerializeField] private float friction;
    [SerializeField] private float frictionMultiplier;
    [SerializeField] private int counterThresold;
    private Vector2Int readyToCounter = Vector2Int.zero;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;
    [SerializeField] private Rigidbody rb;
    public Rigidbody Rb => rb;

    void Awake()
    {
        player.PlayerInput.OnMoveInput += ReceiveMoveInput;
        player.PlayerInput.OnJumpInput += ReceiveJumpInput;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        RelativeVel = player.Orientation.InverseTransformDirection(rb.velocity);

        Friction();
        HoverOffGround();
        UpdateUprightForce();

        float movementMultiplier = 3.5f * Time.fixedDeltaTime * (Grounded ? 1f : 0.6f);
        ClampSpeed(movementMultiplier);

        Vector3 moveDir = player.Orientation.forward * Input.y + player.Orientation.right * Input.x;
        if (InWater) rb.AddForceAtPosition(acceleration * movementMultiplier * moveDir.normalized, transform.position + transform.up, ForceMode.Impulse); 
        else rb.AddForce(acceleration * movementMultiplier * moveDir.normalized, ForceMode.Impulse);

        Magnitude = rb.velocity.magnitude;
        Velocity = rb.velocity;
    }

    private void HoverOffGround()
    {
        Grounded = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out _, rideHeight + rideRayExtension, environment) || InWater;
        bool buffer = Physics.Raycast(transform.position, Vector3.down, out var hit, rideHeight + rideRayExtension * 1.35f, environment);

        if (!buffer || InWater) return;

        Vector3 vel = rb.velocity;
        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = hit.rigidbody;

        if (hitBody != null) otherVel = hitBody.velocity;

        float relVel = Vector3.Dot(Vector3.down, vel) - Vector3.Dot(Vector3.down, otherVel);
        float x = hit.distance - rideHeight;
        float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

        rb.AddForce(Vector3.down * springForce);

        if (hitBody != null) hitBody.AddForceAtPosition(Vector3.down * -springForce, hit.point);
    }

    public void UpdateUprightForce()
    {
        Quaternion fromTo = Quaternion.FromToRotation(transform.up, Vector3.up);
        rb.AddTorque((new Vector3(fromTo.x, fromTo.y, fromTo.z) * uprightSpringStrength) - (rb.angularVelocity * uprightSpringStrengthDamper), ForceMode.Impulse);
    }

    private void Jump()
    {
        if (!Grounded) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        player.CameraBody.CamHeadBob.BobOnce(5f);
    }

    private void Friction()
    {
        if (Jumping || !Grounded) return;

        Vector3 frictionForce = Vector3.zero;

        if (Mathf.Abs(RelativeVel.x) > 0f && Input.x == 0f && readyToCounter.x > counterThresold) frictionForce -= player.Orientation.right * RelativeVel.x;
        if (Mathf.Abs(RelativeVel.z) > 0f && Input.y == 0f && readyToCounter.y > counterThresold) frictionForce -= player.Orientation.forward * RelativeVel.z;

        if (CounterMomentum(Input.x, RelativeVel.x)) frictionForce -= player.Orientation.right * RelativeVel.x;
        if (CounterMomentum(Input.y, RelativeVel.z)) frictionForce -= player.Orientation.forward * RelativeVel.z;

        frictionForce = Vector3.ProjectOnPlane(frictionForce, Vector3.up);
        if (frictionForce != Vector3.zero) rb.AddForce(0.2f * friction * acceleration * frictionForce);

        readyToCounter.x = Input.x == 0f ? readyToCounter.x + 1 : 0;
        readyToCounter.y = Input.y == 0f ? readyToCounter.y + 1 : 0;
    }

    bool CounterMomentum(float input, float mag, float threshold = 0.05f)
        => input > 0 && mag < -threshold || input < 0 && mag > threshold;

    private void ClampSpeed(float movementMultiplier)
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float coefficientOfFriction = acceleration / maxSpeed;

        if (vel.sqrMagnitude > maxSpeed * maxSpeed) rb.AddForce(coefficientOfFriction * movementMultiplier * -vel, ForceMode.Impulse);
    }

    private void ReceiveMoveInput(Vector2 input)
    {
        this.Input = input;
        Moving = input != Vector2.zero;
    }

    private void ReceiveJumpInput(bool jumping)
    {
        Jumping = jumping;
        if (jumping) Jump();
    }
}

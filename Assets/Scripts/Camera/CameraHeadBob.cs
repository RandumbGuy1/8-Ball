using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraHeadBob
{
    [Header("View Bob Settings")]
    [SerializeField] private bool enabled = true;
    [SerializeField] private Vector2 viewBobMultiplier;
    [SerializeField] private float viewBobSpeed;
    [SerializeField] private float viewBobSmoothTime;
    [SerializeField] private float maxTilt;
    [SerializeField] private float tiltSmoothTime;
    private float viewBobTimer = 0f;
    private float landBobOffset = 0f;

    private Vector3 springMotion = Vector3.zero;
    private float tiltSpring = 0f;

    public float TiltSway { get; private set; }
    public Vector3 ViewBobOffset { get; private set; }

    public void BobUpdate(PlayerRef player)
    {
        if (!enabled) return;

        viewBobTimer = player.PlayerMovement.Grounded && (player.PlayerMovement.Magnitude > 0.5f && player.PlayerMovement.Moving)
            ? viewBobTimer + Time.deltaTime : 0f;
        landBobOffset = Mathf.Min(0, landBobOffset + Time.deltaTime * viewBobSmoothTime * 4f);

        float scroller = viewBobTimer * viewBobSpeed;

        float swimMultiplier = player.PlayerMovement.InWater ? 0.25f : 1f;
        Vector3 headBob = swimMultiplier * (viewBobMultiplier.x * Mathf.Cos(scroller) * player.Orientation.right + viewBobMultiplier.y * Mathf.Abs(Mathf.Sin(scroller)) * Vector3.up) 
            + Vector3.down * landBobOffset;
        springMotion = Vector3.Lerp(springMotion, (headBob - ViewBobOffset) * 0.5f, viewBobSmoothTime * Time.deltaTime);

        float tilt = Mathf.Clamp((player.PlayerMovement.Input.x + player.CameraBody.CamLookSettings.RotationDelta.y) * maxTilt, -maxTilt, maxTilt);
        tiltSpring = Mathf.Lerp(tiltSpring, (tilt - TiltSway) * 0.5f, tiltSmoothTime * Time.deltaTime);

        ViewBobOffset += springMotion;
        TiltSway += tiltSpring;
    }

    public void BobOnce(float magnitude)
    {
        if (!enabled) return;

        landBobOffset -= magnitude;
    }

    public void Enable(bool enabled) => this.enabled = enabled;
}

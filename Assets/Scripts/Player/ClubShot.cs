using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubShot : MonoBehaviour
{
    [Header("GFX Settings")]
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Projecton projecton;
    [SerializeField] private GameObject ballGhostPrefb;

    [Header("Shoot Settings")]
    [SerializeField] private LayerMask balls;
    [SerializeField] private GameObject club;
    private IClub equippedClub;

    [SerializeField] private Material unchargedMaterial;
    [SerializeField] private Material chargedMaterial;
    private Rigidbody currentBall;
    private CollisionDetectionMode ballDetection;

    private float chargePower = 0f;
    private float upwardModifier = 0f;
    private float smoothUpwardModifier = 0f;
    private float vel = 0f;
    private SpringJoint joint;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;


    void Awake()
    {
        lr.positionCount = 0;
        player.PlayerInput.OnMouseButtonDownInput += UpdateInput;
        player.PlayerInput.OnMouseButtonInput += CalculateShot;

        if (club != null) equippedClub = club.GetComponent<IClub>();
    }

    private void UpdateInput(int button)
    {
        if (club == null) return;

        if (button == 1) CalculateShot(button);
        if (currentBall != null) return;

        //Check For Balls
        Ray ray = player.PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out var hit, equippedClub.Stats.ClubLength + +(player.transform.position - player.PlayerCam.transform.position).magnitude, balls))
        {
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if (rb == null) return;

            //If clicked mouse button you found a ball
            if (button == 0)
            {
                currentBall = rb;
                ballDetection = currentBall.collisionDetectionMode;

                currentBall.collisionDetectionMode = CollisionDetectionMode.Discrete;
                currentBall.isKinematic = true;
                AddSpring();
            }
        }
    }
    
    void CalculateShot(int button)
    {
        if (currentBall == null) return;

        Vector3 toBall = currentBall.transform.position - player.transform.position;
        toBall.y = 0f;

        float chargeRatio = chargePower / equippedClub.Stats.MaxClubCharge;
        int predictionCount = equippedClub.Stats.PredictionCount;
        if (chargeRatio < 0.01f) predictionCount = Mathf.RoundToInt(predictionCount * 0.1f);

        lr.positionCount = 2 + predictionCount;
        lr.SetPosition(0, currentBall.transform.position - toBall);
        lr.SetPosition(1, currentBall.transform.position);
        lr.startWidth = Mathf.Max(chargeRatio * 0.4f, 0.05f);
        lr.endWidth = lr.startWidth * 0.8f;
        lr.material.Lerp(unchargedMaterial, chargedMaterial, chargeRatio);

        Vector3 shotTrajectory = chargePower * equippedClub.Stats.ClubPower * (toBall.normalized + 1.15f * smoothUpwardModifier * Vector3.up);
        shotTrajectory = Vector3.ClampMagnitude(shotTrajectory, chargePower * equippedClub.Stats.ClubPower);

        upwardModifier = Mathf.Clamp(upwardModifier + Input.mouseScrollDelta.y * 0.1f, -equippedClub.Stats.UpwardClamp, equippedClub.Stats.UpwardClamp);
        smoothUpwardModifier = Mathf.Lerp(smoothUpwardModifier, upwardModifier, 10f * Time.deltaTime);

        if (button == 1)
        {
            RemoveBall(shotTrajectory);
            return;
        }
        else
        {
            float target = button != -1 ? equippedClub.Stats.MaxClubCharge : 0f;
            chargePower = Mathf.SmoothDamp(chargePower, target, ref vel, equippedClub.Stats.ClubChargeTime);
        }

        Vector3[] points = projecton.SimulateTrajectory(ballGhostPrefb, currentBall.transform.position, currentBall.transform.rotation, (Rigidbody rb) => rb.AddForce(shotTrajectory, ForceMode.VelocityChange), predictionCount, chargeRatio);
        for (int i = 0; i < predictionCount; i++) lr.SetPosition(i + 2, points[i]);
    }

    private void AddSpring()
    {
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = currentBall.transform.position;

        joint.maxDistance = equippedClub.Stats.ClubLength;
        joint.minDistance = 0f;

        joint.spring = 5f;
        joint.damper = 10f;
        joint.massScale = 5f;
    }

    private void RemoveBall(Vector3 force)
    {
        currentBall.isKinematic = false;
        currentBall.collisionDetectionMode = ballDetection;
        currentBall.AddForce(force, ForceMode.VelocityChange);
        currentBall = null;

        lr.positionCount = 0;
        lr.startWidth = 0f;
        chargePower = 0f;
        upwardModifier = 0f;

        Destroy(joint);
    }
}

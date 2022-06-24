using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubShot : MonoBehaviour
{
    [Header("GFX Settings")]
    [SerializeField] private LineRenderer lr;
    [SerializeField] private ParticleSystem wind;

    [Header("Shoot Settings")]
    [SerializeField] private LayerMask balls;
    [SerializeField] private float clubLength;
    [SerializeField] private float clubChargeTime;
    [SerializeField] private float maxClubCharge;
    [SerializeField] private float upwardClamp;
    [SerializeField] private float clubPower;

    [SerializeField] private Material unchargedMaterial;
    [SerializeField] private Material chargedMaterial;
    private Rigidbody currentBall;
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
    }

    private void UpdateInput(int button)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            wind.Stop();
            wind.Play();

            Collider[] cols = Physics.OverlapSphere(transform.position, 100f, balls);
            foreach (Collider col in cols)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb == null) continue;

                rb.AddExplosionForce(20f, transform.position, 50f, 1f, ForceMode.Impulse);
                rb.AddForce((player.PlayerCam.transform.forward + Vector3.up) * 10f, ForceMode.Impulse);
            }
        }

        if (button == 1) CalculateShot(button);
        if (currentBall != null) return;

        //Check For balls
        Ray ray = player.PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out var hit, clubLength, balls))
        {
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if (rb == null) return;

            //If clicked mouse button you found a ball
            if (button == 0)
            {
                currentBall = rb;
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

        lr.positionCount = 3;
        lr.SetPosition(0, currentBall.transform.position - toBall);
        lr.SetPosition(1, currentBall.transform.position);

        float chargeRatio = chargePower / maxClubCharge;
        lr.startWidth = Mathf.Max(chargeRatio * 0.4f, 0.05f);
        lr.material.Lerp(unchargedMaterial, chargedMaterial, chargeRatio);

        Vector3 shotTrajectory = chargePower * clubPower * (toBall.normalized + 1.15f * smoothUpwardModifier * Vector3.up);
        shotTrajectory = Vector3.ClampMagnitude(shotTrajectory, chargePower * clubPower);
        
        lr.SetPosition(2, lr.GetPosition(1) + shotTrajectory * 0.6f);

        upwardModifier = Mathf.Clamp(upwardModifier + Input.mouseScrollDelta.y * 0.1f, -upwardClamp, upwardClamp);
        smoothUpwardModifier = Mathf.Lerp(smoothUpwardModifier, upwardModifier, 10 * Time.deltaTime);

        if (button == 1)
        {
            RemoveBall(shotTrajectory);
            return;
        }
        else
        {
            float target = button != -1 ? maxClubCharge : 0f;
            chargePower = Mathf.SmoothDamp(chargePower, target, ref vel, clubChargeTime);
        }
    }

    private void AddSpring()
    {
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = currentBall.transform.position;

        joint.maxDistance = clubLength;
        joint.minDistance = 0f;

        joint.spring = 5f;
        joint.damper = 10f;
        joint.massScale = 5f;
    }

    private void RemoveBall(Vector3 force)
    {
        currentBall.isKinematic = false;
        currentBall.AddForce(force, ForceMode.VelocityChange);
        currentBall = null;

        lr.positionCount = 0;
        lr.startWidth = 0f;
        chargePower = 0f;
        upwardModifier = 0f;

        Destroy(joint);
    }
}

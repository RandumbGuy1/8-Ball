using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubShot : MonoBehaviour
{
    [Header("GFX Settings")]
    [SerializeField] private LineRenderer lr;

    [Header("Shoot Settings")]
    [SerializeField] private LayerMask balls;
    [SerializeField] private float clubLength;
    [SerializeField] private float clubPower;
    private Rigidbody currentBall;
    private Vector3 collisionPoint = Vector3.zero;
    private Vector3 playerBallDelta = Vector3.zero;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        lr.positionCount = 0;
        player.PlayerInput.OnMouseButtonInput += UpdateInput;
    }

    private void UpdateInput(int button)
    {
        //If you have selected a ball calculate its trajectory
        if (currentBall != null)
        {
            CalculateShot(button);
            return;
        }

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

                collisionPoint = hit.point;
                playerBallDelta = collisionPoint - transform.position;

                lr.positionCount = 2;
                lr.SetPosition(0, collisionPoint);
            }
        }
    }

    void CalculateShot(int button)
    {
        //Calculate Force and direction based on distance
        Vector3 pointFromBall = player.PlayerCam.transform.position + (player.PlayerCam.transform.forward * playerBallDelta.magnitude);
        Vector3 pointToBall = collisionPoint - pointFromBall;
        pointToBall.y *= 0.2f;

        lr.SetPosition(1, pointFromBall);
        float mag = pointFromBall.sqrMagnitude;
        lr.startWidth = 0.01f * mag;
        lr.endWidth = 0.01f * mag;

        //If we press left mouse button we shoot, for these other conditions we cancel out the shot
        if (button == 0 || button == 1 || (currentBall.transform.position - transform.position).sqrMagnitude > clubLength * clubLength * 2f)
        {
            currentBall.isKinematic = false;
            if (button == 0) currentBall.AddForceAtPosition(pointToBall * clubPower, collisionPoint, ForceMode.Impulse);

            currentBall = null;
            collisionPoint = Vector3.zero;
            playerBallDelta = Vector3.zero;

            lr.positionCount = 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(collisionPoint, 0.1f);
    }
}

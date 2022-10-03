using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightBallFather : MonoBehaviour
{
    [SerializeField] private float shoveForce = 50f;
    [SerializeField] private Dialogue angryDialogue;
    [SerializeField] private DialogueTrigger trigger;

    public void AlertFather()
    {
        trigger.SetNewDialogue(angryDialogue);
    }

    public void GrabPlayer()
    {
        if (trigger.Player == null) return;

        trigger.Player.PlayerMovement.AddSpring(transform, 1f);
        trigger.Player.CameraBody.CamHeadBob.BobOnce(6f);
    }

    public void ShovePlayer()
    {
        if (trigger.Player == null) return;

        trigger.Player.PlayerMovement.RemoveSpring();

        trigger.Player.PlayerMovement.Rb.velocity = Vector3.zero;
        trigger.Player.ClubHolder.DropClub(false, true);
        trigger.Player.PlayerMovement.GoLimp(1f);
        trigger.Player.PlayerMovement.Rb.AddTorque(20f * shoveForce * Random.insideUnitSphere, ForceMode.VelocityChange);
        trigger.Player.PlayerMovement.Rb.AddExplosionForce(shoveForce, transform.position + Vector3.down, 50f, 1f, ForceMode.VelocityChange);
        trigger.Player.CameraBody.CamHeadBob.BobOnce(8f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightBallFather : MonoBehaviour
{
    [SerializeField] private Dialogue angryDialogue;
    [SerializeField] private DialogueTrigger trigger;

    public void AlertFather(PlayerRef player)
    {
        trigger.SetNewDialogue(angryDialogue);
    }

    public void ShovePlayer(PlayerRef player)
    {
        player.PlayerMovement.GoLimp(0.5f);
        player.PlayerMovement.Rb.AddExplosionForce(40f, transform.position + Vector3.down, 10f, 2f, ForceMode.VelocityChange);
    }
}

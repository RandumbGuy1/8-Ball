using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightBallFather : MonoBehaviour
{
    [SerializeField] private Dialogue angryDialogue;
    [SerializeField] private DialogueTrigger trigger;

    public void AlertFather(PlayerRef player)
    {
        player.PlayerMovement.Rb.AddExplosionForce(30f, transform.position, 5f, 1f, ForceMode.Impulse);
        trigger.SetNewDialogue(angryDialogue);
    }
}

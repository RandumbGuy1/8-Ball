using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildBall : MonoBehaviour, IEightBall
{
    [Header("Event Dialogue")]
    [SerializeField] private Dialogue shittingBricks;
    [SerializeField] private Dialogue hurtDialogue;
    [SerializeField] private Dialogue notSoHurtDialogue;
    [SerializeField] private Dialogue trauma;
    [SerializeField] private EightBallFather father;
    [SerializeField] private DialogueTrigger trigger;
    [SerializeField] private UIAnimationController achievement;

    [Header("Running Away")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float runAwayForce;

    public PlayerRef Player { get; set; }

    public void SelectBall(PlayerRef player)
    {
        player.DialogueHandler.StartConversation(trigger, shittingBricks);
    }

    public void ClubBall(PlayerRef player, float charge)
    {
        if (charge < 0.4f)
        {
            player.DialogueHandler.StartConversation(trigger, notSoHurtDialogue);
            return;
        }

        player.DialogueHandler.StartConversation(trigger, hurtDialogue);
        father.AlertFather();

        trigger.SetNewDialogue(trauma);
    }

    public void RunAwayFromPlayer()
    {
        rb.AddForce((transform.position - Player.transform.position).normalized * runAwayForce, ForceMode.Impulse);
    }

    public void Meme() => StartCoroutine(AchievementMeme());

    private IEnumerator AchievementMeme()
    {
        achievement.HideUI(false);

        yield return new WaitForSeconds(3f);

        achievement.HideUI();
    }
}

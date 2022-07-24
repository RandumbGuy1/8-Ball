using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildBall : MonoBehaviour, IEightBall
{
    [SerializeField] private Dialogue hurtDialogue;
    [SerializeField] private Dialogue trauma;
    [SerializeField] private EightBallFather father;
    [SerializeField] private DialogueTrigger trigger;
    [SerializeField] private UIAnimationController achievement;

    public void ClubBall(PlayerRef player)
    {
        player.DialogueHandler.StartConversation(trigger, hurtDialogue);
        father.AlertFather(player);

        trigger.SetNewDialogue(trauma);
    }

    public void Meme() => StartCoroutine(AchievementMeme());

    private IEnumerator AchievementMeme()
    {
        achievement.HideUI(false);

        yield return new WaitForSeconds(3f);

        achievement.HideUI();
    }
}

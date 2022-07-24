using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildBall : MonoBehaviour, IEightBall
{
    [SerializeField] private EightBallFather father;

    public void ClubBall(PlayerRef player)
    {
        father.AlertFather(player);
    }
}

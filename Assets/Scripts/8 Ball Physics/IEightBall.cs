using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEightBall
{
    PlayerRef Player { get; set; }
    void SelectBall(PlayerRef player);
    void ClubBall(PlayerRef player, float charge);
}

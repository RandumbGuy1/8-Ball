using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicClub : MonoBehaviour, IClub
{
    [SerializeField] private ClubStats stats = new ClubStats();
    public ClubStats Stats => stats;

    public void ThrustBalls(PlayerRef player)
    {
        print(player + ": Thrusted my balls :(");
    }

    public void UseClubAbility(PlayerRef player)
    {
        print(player + ": Used club ability on my balls :(");
    }

    public void UseMovementAbility(PlayerRef player)
    {
        print(player + ": Used movement ability on my balls :(");
    }
}

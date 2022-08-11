using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    [SerializeField] List<Monologue> monologues = new List<Monologue>();
    public List<Monologue> Monologues => monologues;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogueSection
{
    int Intensity { get; }
    string ReceievePrompt();
}

[System.Serializable]
public class Dialogue 
{
    [SerializeField] private string name;
    [SerializeField] List<Monologue> monologues = new List<Monologue>();

    public string Name => name;
    public List<Monologue> Monologues => monologues;
}

[System.Serializable]
public class Monologue : IDialogueSection
{
    [SerializeField] private int intensity;
    [TextArea(3, 10)] [SerializeField] private string openPrompt;
    public int Intensity => intensity;
    public string ReceievePrompt() => openPrompt;
}


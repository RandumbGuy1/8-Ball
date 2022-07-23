using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogueSection
{
    string ReceievePrompt();
    void Accept();
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
    [TextArea(3, 10)]
    [SerializeField] private string openPrompt;

    public string ReceievePrompt() => openPrompt;
    public void Accept() { }
}


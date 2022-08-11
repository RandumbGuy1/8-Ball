using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    [SerializeField] private string name;
    [SerializeField] List<Monologue> monologues = new List<Monologue>();

    public string Name => name;
    public List<Monologue> Monologues => monologues;
}

public interface IDialogueSection
{
    UnityEvent DialogueAction { get; }

    int Intensity { get; }
    string ReceievePrompt();
    string[] ReceievePrompts();
}


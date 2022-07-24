using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDialogueSection
{
    UnityEvent DialogueAction { get; }

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
    [TextArea(3, 10)] [SerializeField] private string openPrompt;
    [SerializeField] private int intensity;
    [SerializeField] private UnityEvent dialogueAction;

    public int Intensity => intensity;
    public string ReceievePrompt() => openPrompt;
    public UnityEvent DialogueAction => dialogueAction;
}


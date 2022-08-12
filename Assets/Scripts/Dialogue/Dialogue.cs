using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialogue
{
    [SerializeField] private Author author;
    [SerializeField] private List<Monologue> monologues = new List<Monologue>();
    [SerializeField] private List<DialogueAction> dialogueActions = new List<DialogueAction>();

    public List<Monologue> Monologues => monologues;
    public Author Author => author;

    public DialogueAction FindAction(Monologue node)
    {
        foreach (DialogueAction search in dialogueActions)
            if (node == search.MonologueKey) return search;

        return null;
    }
}

[System.Serializable]
public class DialogueAction
{
    [SerializeField] private DialougeActionTime time;
    [SerializeField] private Monologue monologueKey;
    [SerializeField] private UnityEvent monologueEvent;

    public DialougeActionTime Time => time;
    public Monologue MonologueKey => monologueKey;
    public UnityEvent MonologueEvent => monologueEvent;
}

public enum DialougeActionTime
{
    Start,
    End
};


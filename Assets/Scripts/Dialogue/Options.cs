using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Options", menuName = "Custom Speech Option Data", order = 1)]
public class Options : ScriptableObject
{
    [SerializeField] private string[] optionsTexts = new string[2];
    [SerializeField] private Branch[] dialogueContinuations = new Branch[2];

    public string[] OptionTexts => optionsTexts;
    public Branch[] DialogueContinuations => dialogueContinuations;
}

[System.Serializable]
public class Branch
{
    [SerializeField] private List<Monologue> monologues = new List<Monologue>();
    public List<Monologue> Monologues => monologues;
}

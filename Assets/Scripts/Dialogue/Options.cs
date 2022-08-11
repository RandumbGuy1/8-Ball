using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monologue", menuName = "Custom Speech Option Data", order = 1)]
public class Options : ScriptableObject
{
    [SerializeField] private string[] optionsTexts = new string[2];
    [SerializeField] private Dialogue[] dialogueContinuations = new Dialogue[2];

    public string[] OptionTexts => optionsTexts;
    public Dialogue[] DialogueContinuations => dialogueContinuations;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monologue", menuName = "Custom Speech Option Data", order = 1)]
public class Options : ScriptableObject
{
    [SerializeField] private string[] optionsTexts = new string[2];
    [SerializeField] private Monologue[] monologueContinuations = new Monologue[2];

    public string[] OptionTexts => optionsTexts;
    public Monologue[] MonologueContinuations => monologueContinuations;
}

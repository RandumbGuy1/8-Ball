using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monologue", menuName = "Custom Speech Data", order = 1)]
public class Monologue : ScriptableObject
{
    [SerializeField] private Options branch;
    [TextArea(3, 10)] [SerializeField] private string openPrompt;
    [SerializeField] private int intensity;
    [SerializeField] private UnityEvent dialogueAction;

    public Options Branch => branch;
    public int Intensity => intensity;
    public string OpenPrompt => openPrompt;
    public UnityEvent DialogueAction => dialogueAction;
}

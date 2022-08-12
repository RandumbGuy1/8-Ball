using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monologue", menuName = "Custom Speech Data", order = 1)]
public class Monologue : ScriptableObject
{
    [SerializeField] private Options branch;
    [TextArea(3, 10)] [SerializeField] private string openPrompt;
    [SerializeField] private int intensity;
    [SerializeField] private Emotion authorEmotion;

    public Options Branch => branch;
    public int Intensity => intensity;
    public string OpenPrompt => openPrompt;
    public Emotion AuthorEmotion => authorEmotion;

    public void SetEmotion(Emotion newEmotion) => authorEmotion = newEmotion;
}

public enum Emotion
{
    Happy,
    Sad,
    Angry,
    Normal,
    Scared,
    Death
};

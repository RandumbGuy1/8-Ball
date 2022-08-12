using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Author", menuName = "Custom Author Data", order = 1)]
public class Author : ScriptableObject
{
    [SerializeField] private string authorName;
    [SerializeField] private Sprite authorProfile;

    public string AuthorName => authorName;
    public Sprite AuthorProfile => authorProfile;
}

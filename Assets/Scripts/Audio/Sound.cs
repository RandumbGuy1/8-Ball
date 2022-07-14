using UnityEngine;

[System.Serializable]
public class Sound 
{
    [SerializeField] private AudioClip clip;
    [SerializeField, Range(0, 1)] private float volume;
    [SerializeField, Range(0, 1)] private float pitch;
    [SerializeField] private bool looping;
    public AudioSource Source { get; set; } = null;

    public AudioClip Clip => clip;
    public float Volume { get => volume; set => volume = value; }
    public float Pitch { get => pitch; set => pitch = value; } 
    public bool Looping { get => looping; set => looping = value; } 

    public void SetParamaters()
    {
        if (Source == null) return;

        Source.clip = clip;
        Source.volume = volume;
        Source.pitch = pitch;
        Source.loop = looping;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    [SerializeField] private AudioClip[] soundsToPlayOnAwake;

    private Dictionary<AudioClip, Sound> soundDictionary = new Dictionary<AudioClip, Sound>();

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.SetParamaters();

            soundDictionary.Add(sound.Clip, sound);
        }

        foreach (AudioClip clip in soundsToPlayOnAwake) PlayOnce(clip);
    }

    public void PlayOnce(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (!soundDictionary.ContainsKey(clip) || (soundDictionary[clip].Looping && soundDictionary[clip].Source.isPlaying)) return;

        float tempVolume = soundDictionary[clip].Volume;
        soundDictionary[clip].Volume *= volumeMultiplier;

        soundDictionary[clip].SetParamaters();
        soundDictionary[clip].Source.Play();

        soundDictionary[clip].Volume = tempVolume;
    }

    public void StopSound(AudioClip clip)
    {
        if (!soundDictionary.ContainsKey(clip)) return;
        soundDictionary[clip].Source.Stop();
    }

    public bool IsPlaying(AudioClip clip)
    {
        if (!soundDictionary.ContainsKey(clip)) return false;
        return soundDictionary[clip].Source.isPlaying;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject soundInstancePrefab;
    [SerializeField] private Sound[] sounds;

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

        //Store all sounds in a dictionary
        foreach (Sound sound in sounds)
        {
            Queue<AudioSource> soundInstances = new Queue<AudioSource>();
            for (int i = 0; i < sound.SoundCapacity; i++)
            {
                AudioSource instance = Instantiate(soundInstancePrefab).GetComponent<AudioSource>();
                soundInstances.Enqueue(instance);
            }

            soundDictionary.Add(sound.Clip, sound);
            sound.SetSourceQueue(soundInstances);
            
            //Play all sounds set to playOnAwake
            if (!sound.PlayOnAwake) continue;
            PlayOnce(sound.Clip, Vector3.zero);
        }
    }

    public AudioSource PlayOnce(AudioClip clip, Vector3 sourcePos, float volumeMultiplier = 1f)
    {
        if (!soundDictionary.ContainsKey(clip)) return null;

        //Spawn audio instance at the sounds source position
        AudioSource source = soundDictionary[clip].SourcesQueue.Dequeue();
        if (source == null) return null;

        //Set the data of the audio source using the sound class preset
        soundDictionary[clip].SetParamaters(source);
        source.volume *= volumeMultiplier;

        source.Play();

        //Send sound back to the sounds pool
        soundDictionary[clip].SourcesQueue.Enqueue(source);

        return source;
    }

    //Stop a certain audio source
    public void StopSound(AudioClip clip, AudioSource source)
    {
        if (!soundDictionary.ContainsKey(clip)) return;
        if (source == null) return;

        soundDictionary[clip].StopInstance(source);
    }

    //Stop all audio of a certain sound
    public void StopAllSounds(AudioClip clip)
    {
        if (!soundDictionary.ContainsKey(clip)) return;

        soundDictionary[clip].StopAllAudio();
    }
}

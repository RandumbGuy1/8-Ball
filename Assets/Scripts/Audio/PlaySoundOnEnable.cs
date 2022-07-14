using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    [SerializeField] bool playOnAwake = false;
    [SerializeField] private AudioSource source;
    [SerializeField] private ParticleSystem particleDependancy;

    void OnEnable()
    {
        if (!playOnAwake)
        {
            playOnAwake = true;
            return;
        }

        source.Play();
    }

    void OnDisable()
    {
        source.Stop();
    }
}

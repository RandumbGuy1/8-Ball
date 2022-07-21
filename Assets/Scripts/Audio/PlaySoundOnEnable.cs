using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] bool playOnAwake = false;
    [SerializeField] private float volumeMultiplier;
    [SerializeField] private AudioClip source;

    void OnEnable()
    {
        if (!playOnAwake)
        {
            playOnAwake = true;
            return;
        }

        AudioManager.Instance.PlayOnce(source, transform.position, volumeMultiplier);
    }
}

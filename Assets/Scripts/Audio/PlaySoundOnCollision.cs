using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.PlayOnce(clip, transform.position);
    }
}

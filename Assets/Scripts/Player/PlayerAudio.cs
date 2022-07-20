using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Water Audio")]
    [SerializeField] private AudioClip underWaterClip;
    [SerializeField] private AudioClip waterMovementClip;

    private AudioSource waterMovementSource = null;
    private AudioSource underWaterSource = null;

    Dictionary<AudioSource, float> storedVolumes = new Dictionary<AudioSource, float>();

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        player.PlayerMovement.OnWaterEnter += PlayUnderWaterAmbience;
        player.PlayerInput.OnMoveInput += UpdateWaterMovementSounds;
    }

    private void UpdateWaterMovementSounds(Vector2 input)
    {
        if (waterMovementSource != null && storedVolumes.ContainsKey(waterMovementSource))
        {
            float targetVolume = storedVolumes[waterMovementSource] * (input == Vector2.zero ? 0.3f : 1f);
            waterMovementSource.volume = targetVolume;
        }

        if (underWaterSource != null && storedVolumes.ContainsKey(underWaterSource))
        {
            float targetVolume = storedVolumes[underWaterSource] * (player.PlayerMovement.Submergence > 0.85f ? 1f : 0.2f);
            underWaterSource.volume = targetVolume;
        }
    }

    private void PlayUnderWaterAmbience(bool submerged)
    {
        if (submerged)
        {
            waterMovementSource = AudioManager.Instance.PlayOnce(waterMovementClip, Vector3.zero);
            underWaterSource = AudioManager.Instance.PlayOnce(underWaterClip, Vector3.zero);

            storedVolumes.Add(waterMovementSource, waterMovementSource.volume);
            storedVolumes.Add(underWaterSource, underWaterSource.volume);
            return;
        }

        AudioManager.Instance.StopAllSounds(waterMovementClip);
        AudioManager.Instance.StopAllSounds(underWaterClip);
        waterMovementSource = null;

        storedVolumes.Clear();
    }
}

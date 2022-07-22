using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
    [Header("Grounded Movement Audio")]
    [SerializeField] private AudioClip playerLandClip;
    [SerializeField] private float footStepFrequency;
    [SerializeField] private float footStepVolumeMultiplier;
    [Space]
    [SerializeField] private AudioClip playerCrouchClip;
    private float footstepDistance;

    [Header("Water Audio")]
    [SerializeField] private AudioClip underWaterClip;
    [SerializeField] private AudioClip waterMovementClip;

    [Header("Audio Filtering")]
    [SerializeField] private AudioDistortionFilter distortionFilter;
    [SerializeField] private AudioLowPassFilter lowPassFilter;

    private AudioSource waterMovementSource = null;
    private AudioSource underWaterSource = null;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        GameManager.Instance.OnGameStateChanged += ToggleDistortion;

        player.PlayerMovement.OnWaterEnter += PlayUnderWaterAmbience;
        player.PlayerInput.OnMoveInput += UpdateWaterMovementSounds;
        player.PlayerInput.OnMoveInput += CalculateFootsteps;

        player.PlayerMovement.OnPlayerLand += (float magnitude) => { AudioManager.Instance.PlayOnce(playerLandClip, transform.position); };
        player.PlayerMovement.OnPlayerMove += (bool input) => {
            if (!player.PlayerMovement.Grounded || input) return;
            AudioManager.Instance.PlayOnce(playerLandClip, transform.position, footStepVolumeMultiplier); 
        };

        player.PlayerMovement.OnPlayerCrouch += (bool input) => 
        {
            if (!player.PlayerMovement.Grounded) return;
            AudioManager.Instance.PlayOnce(playerCrouchClip, transform.position);         
        };

        ToggleDistortion(GameState.Gameplay);
    }

    private void UpdateWaterMovementSounds(Vector2 input)
    {
        if (waterMovementSource != null)
        {
            float targetVolume = AudioManager.Instance.SoundDictionary[waterMovementClip].Volume * (input == Vector2.zero ? 0.4f : 0.8f);
            waterMovementSource.volume = targetVolume;
        }

        if (underWaterSource != null)
        {
            float targetVolume = AudioManager.Instance.SoundDictionary[underWaterClip].Volume * (player.PlayerMovement.Submergence > 0.95f ? 1f : 0.1f);
            underWaterSource.volume = targetVolume;
        }
    }

    private void PlayUnderWaterAmbience(bool submerged)
    {
        if (submerged)
        {
            waterMovementSource = AudioManager.Instance.PlayOnce(waterMovementClip, Vector3.zero);
            underWaterSource = AudioManager.Instance.PlayOnce(underWaterClip, Vector3.zero);
            return;
        }

        AudioManager.Instance.StopAllSounds(waterMovementClip);
        AudioManager.Instance.StopAllSounds(underWaterClip);
        waterMovementSource = null;
    }

    private void CalculateFootsteps(Vector2 input)
    {
        if (!player.CameraBody.CamHeadBob.Bobbing)
        {
            footstepDistance = 0f;
            return;
        }

        float walkMagnitude = player.PlayerMovement.Magnitude;
        walkMagnitude = Mathf.Clamp(walkMagnitude, 0f, 20f);

        footstepDistance += walkMagnitude * Time.deltaTime * footStepFrequency;

        if (footstepDistance > 450f)
        {
            AudioManager.Instance.PlayOnce(playerLandClip, transform.position, footStepVolumeMultiplier);
            footstepDistance = 0f;
        }
    }

    private void ToggleDistortion(GameState newState)
    {
        if (distortionFilter == null || lowPassFilter == null) return;

        distortionFilter.enabled = newState == GameState.Paused;
        lowPassFilter.enabled = newState == GameState.Paused;
    }
}

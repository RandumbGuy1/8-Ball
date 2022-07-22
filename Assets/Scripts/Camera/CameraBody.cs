using UnityEngine;
using UnityEngine.Rendering;

public class CameraBody : MonoBehaviour
{
    [SerializeField] private CameraFOV camFov;
    [SerializeField] private CameraIdleSway camIdleSway;
    [SerializeField] private CameraHeadBob camHeadBob;
    [SerializeField] private TPSCameraCollider camCollider;
    [SerializeField] private CameraLook camLookSettings;
    [SerializeField] private CameraSprintEffect camSprintEffect;

    [Header("UnderWater Filter Settings")]
    [SerializeField] private Volume regularVolume;
    [SerializeField] private Volume underWaterVolume;
    [SerializeField] private float filterSmoothTime;
    private float targetWeight = 0f;

    [Header("Regular Settings")]
    [SerializeField] private Vector3 posOffset;
    private Vector3 smoothPosOffset = Vector3.zero;

    public Volume UnderWaterVolume => underWaterVolume;
    public CameraIdleSway CamIdleSway => camIdleSway;
    public CameraHeadBob CamHeadBob => camHeadBob;
    public TPSCameraCollider CamCollider => camCollider;
    public CameraLook CamLookSettings => camLookSettings;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        SetCursorState(true);

        player.PlayerInput.OnMouseInput += camLookSettings.LookUpdate;
        player.PlayerInput.OnPerspectiveToggle += (bool toggle) => {
            if (!toggle) return;

            camCollider.Enabled = !camCollider.Enabled;
            player.Rendering.shadowCastingMode = camCollider.Enabled ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
        };

        player.PlayerMovement.OnPlayerLand += camHeadBob.BobOnce;
    }

    void Update()
    {
        player.PlayerCam.fieldOfView = camFov.FOVUpdate(player);

        camSprintEffect.SpeedLines(player);
        camIdleSway.IdleCameraSway(player);
        camHeadBob.BobUpdate(player);
        camCollider.ColliderUpdate(player.PlayerCam.transform.position, player.transform.position);
    }

    void LateUpdate()
    {
        //Apply Rotations And Positions
        {
            Vector3 externalPlayerRotation = new Vector3(player.PlayerMovement.Rb.rotation.x, player.PlayerMovement.Rb.rotation.y, player.PlayerMovement.Rb.rotation.z);
            Quaternion newCamRot = Quaternion.Euler((Vector3) camLookSettings.SmoothRotation + ToEuler(camHeadBob.ViewBobOffset) + Vector3.forward * camHeadBob.TiltSway + camIdleSway.HeadSwayOffset + externalPlayerRotation);
            Quaternion newPlayerRot = Quaternion.Euler(0f, camLookSettings.SmoothRotation.y, 0f);

            player.Orientation.localRotation = newPlayerRot;
            transform.localRotation = newCamRot;

            Vector3 cameraTPSOffset = camCollider.Enabled ? posOffset : new Vector3(0f, 0.5f, 0f);
            smoothPosOffset = Vector3.Lerp(smoothPosOffset, cameraTPSOffset, 6f * Time.deltaTime);

            player.PlayerCam.transform.localPosition = Vector3.back * camCollider.SmoothPull + camHeadBob.ViewBobOffset * 0.135f + player.PlayerMovement.CrouchOffset + smoothPosOffset;
            transform.position = player.transform.position;
        }

        //Update Underwater filter
        {
            if (Mathf.Abs(targetWeight - underWaterVolume.weight) < 0.01f)
            {
                underWaterVolume.weight = targetWeight;
                return;
            }

            underWaterVolume.weight = Mathf.Lerp(underWaterVolume.weight, targetWeight, filterSmoothTime * Time.deltaTime);
        }
    }

    public void SetUnderWaterVolumeWeight(float value) => targetWeight = value;

    public void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private Vector3 ToEuler(Vector3 a)
    {
        return new Vector3(a.y, a.x, a.z);
    }
}

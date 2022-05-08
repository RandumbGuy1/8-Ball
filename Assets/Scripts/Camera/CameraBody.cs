using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBody : MonoBehaviour
{
    [SerializeField] private TPSCameraCollider camCollider;
    [SerializeField] private CameraLook camLookSettings;
    [SerializeField] private Vector3 posOffset;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;

    void Awake()
    {
        SetCursorState(true);
        player.PlayerInput.OnMouseInput += camLookSettings.LookUpdate;
        player.PlayerInput.OnPerspectiveToggle += (bool toggle) => { if (toggle) camCollider.Enabled = !camCollider.Enabled; };
    }

    void LateUpdate()
    {
        camCollider.ColliderUpdate(player.PlayerCam.transform.position, player.transform.position);

        //Apply Rotations And Positions
        {
            Quaternion newCamRot = Quaternion.Euler(camLookSettings.SmoothRotation);
            Quaternion newPlayerRot = Quaternion.Euler(0f, camLookSettings.SmoothRotation.y, 0f);

            player.Orientation.localRotation = newPlayerRot;
            transform.localRotation = newCamRot;

            player.PlayerCam.transform.localPosition = Vector3.back * camCollider.SmoothPull;
            transform.position = player.transform.position + posOffset;
        }
    }

    public void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnDrawGizmosSelected()
    {
        if (camCollider != null) camCollider.OnDrawGizmosSelected(player.PlayerCam.transform.position);
    }
}

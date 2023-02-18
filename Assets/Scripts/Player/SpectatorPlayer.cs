using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SpectatorPlayer : MonoBehaviour
{
    [SerializeField] private GenerateUIItemColumns generateItems;
    [SerializeField] private GameObject[] buildPrefabs;
    [SerializeField] private float spectateSpeed;
    [SerializeField] private PlayerRef player;
    private bool lockedCursor = false;

    void Awake()
    {
        player.PlayerInput.OnMouseButtonDownInput += PanCamera;
        player.PlayerInput.OnJumpHoldInput += UpdateMovement;
        GameManager.Instance.OnGameStateChanged += EnablePlayer;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= EnablePlayer;
    }

    private void EnablePlayer(GameState state)
    {
        if (state == GameState.Paused) return;

        player.PlayerMovement.enabled = state == GameState.Gameplay;
        player.PlayerMovement.Rb.isKinematic = state == GameState.Editor;
        player.PlayerMovement.Rb.detectCollisions = state == GameState.Gameplay;

        if (state == GameState.Editor)
        {
            generateItems.Generate(buildPrefabs, this);
            lockedCursor = true;
            player.CameraBody.SetCursorState(lockedCursor);
            player.CameraBody.SetMoveCamera(lockedCursor);
        }
        else
        {
            player.CameraBody.SetCursorState(true);
            player.CameraBody.SetMoveCamera(true);
        }
    }

    void PanCamera(int mouseButton)
    {
        GameState state = GameManager.Instance.CurrentGameState;
        if (state != GameState.Editor || mouseButton != 2) return;

        lockedCursor = !lockedCursor;
        player.CameraBody.SetCursorState(lockedCursor);
        player.CameraBody.SetMoveCamera(lockedCursor);
    }

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    public void SelectItem(GameObject prefab)
    {
        Instantiate(prefab, transform.position + player.PlayerCam.transform.forward * 10f, transform.rotation);
    }

    void UpdateMovement(bool jumping)
    {
        GameState state = GameManager.Instance.CurrentGameState;
        if (state != GameState.Editor) return;

        Vector3 moveOffset = 0.01f * spectateSpeed * player.Orientation.TransformDirection(player.PlayerMovement.Input.x, 0, player.PlayerMovement.Input.y);
        if (jumping) moveOffset += 0.01f * spectateSpeed * Vector3.up;
        if (player.PlayerMovement.Crouching) moveOffset -= 0.01f * spectateSpeed * Vector3.up;

        player.transform.position = player.transform.position + moveOffset;
    }
}

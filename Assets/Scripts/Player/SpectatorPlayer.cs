using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorPlayer : MonoBehaviour
{
    [SerializeField] private float spectateSpeed;
    [SerializeField] private PlayerRef player;

    void Awake()
    {
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

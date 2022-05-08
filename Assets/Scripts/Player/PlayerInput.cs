using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public delegate void ReceiveMoveInput(Vector2 input);
    public event ReceiveMoveInput OnMoveInput;

    public delegate void ReceiveMouseInput(Vector2 input);
    public event ReceiveMouseInput OnMouseInput;

    public delegate void ReceiveJump(bool jump);
    public event ReceiveJump OnJumpInput;

    public delegate void ReceiveToggle(bool toggle);
    public event ReceiveToggle OnPerspectiveToggle;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode togglePerspectKey;

    void Update()
    {
        OnMoveInput?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        OnMouseInput?.Invoke(new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X")));
        OnJumpInput?.Invoke(Input.GetKeyDown(jumpKey));
        OnPerspectiveToggle?.Invoke(Input.GetKeyDown(togglePerspectKey));
    }
}

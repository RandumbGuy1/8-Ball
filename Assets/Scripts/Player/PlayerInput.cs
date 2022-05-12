using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public delegate void ReceiveMoveInput(Vector2 input);
    public event ReceiveMoveInput OnMoveInput;

    public delegate void ReceiveMouseInput(Vector2 input);
    public event ReceiveMouseInput OnMouseInput;

    public delegate void ReceiveJump(bool jumping);
    public event ReceiveJump OnJumpInput;

    public delegate void ReceiveToggle(bool toggle);
    public event ReceiveToggle OnPerspectiveToggle;

    public delegate void ReceieveMouseButton(int button);
    public event ReceieveMouseButton OnMouseButtonInput;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode togglePerspectKey;

    void Update()
    {
        OnMoveInput?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        OnMouseInput?.Invoke(new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X")));
        OnJumpInput?.Invoke(Input.GetKeyDown(jumpKey));
        OnPerspectiveToggle?.Invoke(Input.GetKeyDown(togglePerspectKey));
        OnMouseButtonInput?.Invoke(MouseButton());
    }

    int MouseButton()
    {
        if (Input.GetMouseButtonDown(0)) return 0;
        if (Input.GetMouseButtonDown(1)) return 1;
        if (Input.GetMouseButtonDown(2)) return 2;
        return -1;
    }
}

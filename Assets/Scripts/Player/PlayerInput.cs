using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public delegate void ReceieveVector2Input(Vector2 input);
    public delegate void ReceieveBoolInput(bool input);
    public delegate void ReceieveIntInput(int input);

    public event ReceieveVector2Input OnMoveInput;
    public event ReceieveVector2Input OnMouseInput;

    public event ReceieveBoolInput OnJumpInput;
    public event ReceieveBoolInput OnSwimSinkInput;
    public event ReceieveBoolInput OnCrouchInput;
    public event ReceieveBoolInput OnInteractInput;

    public event ReceieveIntInput OnAbilitySelectInput;
    public event ReceieveIntInput OnClubSelectInput;
    public event ReceieveBoolInput OnClubDropInput;

    public event ReceieveBoolInput OnPerspectiveToggle;
    public event ReceieveBoolInput OnPauseToggle;

    public event ReceieveIntInput OnMouseButtonDownInput;
    public event ReceieveIntInput OnMouseButtonInput;

    [Header("Movement Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode crouchKey;
    [SerializeField] private KeyCode sinkSwimKey;

    [Header("Interact Keybinds")]
    [SerializeField] private KeyCode interactKey;

    [Header("Club Keybinds")]
    [SerializeField] private KeyCode dropClubKey;
    [SerializeField] private List<KeyCode> clubAbilityKeys = new List<KeyCode>();
    [SerializeField] private List<KeyCode> clubSwitchKeys = new List<KeyCode>();

    [Header("UI Keybinds")]
    [SerializeField] private KeyCode togglePerspectKey;
    [SerializeField] private KeyCode pauseMenuKey;

    public KeyCode JumpKey => jumpKey;
    public KeyCode CrouchKey => crouchKey;
    public KeyCode SinkSwimKey => sinkSwimKey;
    public KeyCode InteractKey => interactKey;
    public KeyCode TogglePerspectKey => togglePerspectKey;
    public KeyCode PauseMenuKey => pauseMenuKey;

    void Update()
    {
        OnPauseToggle?.Invoke(Input.GetKeyDown(pauseMenuKey));

        if (GameManager.Instance.CurrentGameState == GameState.Paused) return;

        OnMoveInput?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        OnMouseInput?.Invoke(new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X")));

        OnJumpInput?.Invoke(Input.GetKeyDown(jumpKey));
        OnCrouchInput?.Invoke(Input.GetKey(crouchKey));
        OnSwimSinkInput?.Invoke(Input.GetKey(sinkSwimKey));
        OnInteractInput?.Invoke(Input.GetKeyDown(interactKey));
        OnClubDropInput?.Invoke(Input.GetKeyDown(dropClubKey));
        OnClubSelectInput?.Invoke(IterateKeyBinds(clubSwitchKeys));
        OnAbilitySelectInput?.Invoke(IterateKeyBinds(clubAbilityKeys));

        OnPerspectiveToggle?.Invoke(Input.GetKeyDown(togglePerspectKey));

        OnMouseButtonDownInput?.Invoke(MouseButtonDown());
        OnMouseButtonInput?.Invoke(MouseButton());
    }

    int IterateKeyBinds(List<KeyCode> keys)
    {
        foreach (KeyCode key in keys) if (Input.GetKeyDown(key)) return keys.IndexOf(key);
        return -1;
    }

    int MouseButton()
    {
        if (Input.GetMouseButton(0)) return 0;
        if (Input.GetMouseButton(1)) return 1;
        if (Input.GetMouseButton(2)) return 2;
        return -1;
    }

    int MouseButtonDown()
    {
        if (Input.GetMouseButtonDown(0)) return 0;
        if (Input.GetMouseButtonDown(1)) return 1;
        if (Input.GetMouseButtonDown(2)) return 2;
        return -1;
    }
}

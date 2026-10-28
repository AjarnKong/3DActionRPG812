using UnityEngine;
/// <summary>
/// Reads raw values from the Input System and exposes them as simple properties.
/// Keeping input separated from movement logic makes both easier to test and reuse.
/// </summary>
/// 
[DefaultExecutionOrder(-10)]
public class PlayerInputReader : MonoBehaviour
{
    // Movement direction from the W A S D keys, ranging from -1 to 1 on both axes
    public Vector2 MoveInput { get; private set; }

    // Mouse movement delta for this frame, used to rotate the camera
    public Vector2 LookInput { get; private set; }

    // Mouse scroll wheel value, positive when scrolling up
    public float ZoomInput { get; private set; }

    // True while the sprint key is held down
    public bool SprintHeld { get; private set; }

    // True only on the single frame the jump key was pressed
    public bool JumpPressed { get; private set; }

    // True only on the single frame the attack button was pressed 
    public bool AttackPressed { get; private set; }

    private GameInput _gameInput;

    private void Awake()
    {
        // GameInput is generated automatically from the .inputactions asset
        _gameInput = new GameInput();
    }
    private void OnEnable()
    {
        // Input is only collected while this component is active
        _gameInput.Player.Enable();
    }
    private void OnDisable()
    {
        // Always disable the action map so input never leaks into menus
        _gameInput.Player.Disable();
    }
    private void Update()
    {
        MoveInput = _gameInput.Player.Move.ReadValue<Vector2>();
        LookInput = _gameInput.Player.Look.ReadValue<Vector2>();
        ZoomInput = _gameInput.Player.Zoom.ReadValue<float>();
        SprintHeld = _gameInput.Player.Sprint.IsPressed();
        JumpPressed = _gameInput.Player.Jump.WasPressedThisFrame();
        AttackPressed = _gameInput.Player.Attack.WasPressedThisFrame();
    }
}
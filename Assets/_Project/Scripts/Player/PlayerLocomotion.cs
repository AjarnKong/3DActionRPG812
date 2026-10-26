using UnityEngine;
using System;
/// <summary>
/// Moves the character with a CharacterController using camera relative input.
/// Gravity and jumping are handled manually because CharacterController
/// does not respond to physics forces.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float speedChangeRate = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Header("Gravity and Jump")]
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float groundedStickForce = -2f;

    private CharacterController _controller;
    private PlayerInputReader _input;
    private Transform _cameraTransform;

    private float _currentSpeed;
    private float _verticalVelocity;
    private float _rotationVelocity;

    // Current movement speed, used to drive the locomotion blend tree 
    public float CurrentSpeed => _currentSpeed;

    // True while the capsule is touching the ground 
    public bool IsGrounded => _controller.isGrounded;

    // Negative while falling, positive right after a jump 
    public float VerticalVelocity => _verticalVelocity;

    // Raised once at the exact frame a jump starts 
    public event Action OnJumped;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputReader>();
        // Cache the camera transform instead of calling Camera.main every frame
        _cameraTransform = Camera.main.transform;
    }
    private void Update()
    {
        ApplyGravity();
        HandleJump();
        HandleMovement();
    }

    /// <summary>
    /// Converts the 2D input into a world space direction relative to the camera,
    /// then rotates and moves the character towards that direction.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 input = _input.MoveInput;
        float targetSpeed = _input.SprintHeld ? sprintSpeed : walkSpeed;

        // Releasing every key should bring the character to a stop
        if (input == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        // Accelerate gradually so the character never starts or stops instantly
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

        Vector3 moveDirection = Vector3.zero;

        if (input != Vector2.zero)
        {
            // Atan2 turns the input vector into an angle, and the camera angle is added

            // so that pressing W always walks away from the camera
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg
            + _cameraTransform.eulerAngles.y;

            // Rotate towards the target angle smoothly instead of snapping to it
            float smoothedAngle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y, targetAngle,
            ref _rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // Build the world space direction from the unsmoothed target angle
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // Horizontal movement and vertical velocity are combined into a single call
        Vector3 velocity = moveDirection * _currentSpeed + Vector3.up * _verticalVelocity;

        _controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Accumulates downward velocity every frame while the character is in the air.
    /// </summary>
    private void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            // A small downward force keeps the controller pressed to the ground
            _verticalVelocity = groundedStickForce;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Applies an upward velocity when the jump key is pressed while grounded.
    /// </summary>
    private void HandleJump()
    {
        if (_controller.isGrounded && _input.JumpPressed)
        {
            // v = sqrt(h * -2 * g) is the velocity required to reach the desired height
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // Notify listeners such as the animator controller 
            OnJumped?.Invoke();
        }
    }
}
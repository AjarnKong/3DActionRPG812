using UnityEngine;

/// <summary> 
/// Bridges the movement system and the Animator. 
/// This script never calculates movement, it only reports the current state. 
/// </summary> 
[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLocomotion locomotion;

    //Commented out to avoid errors since FootstepAudio is not defined 
    [SerializeField] private FootstepAudio footstepAudio; 

    [Header("Tuning")]
    [SerializeField] private float speedDampTime = 0.1f;
    [SerializeField] private float fallThreshold = -2.5f;

    // Hashing the parameter names once avoids repeated string lookups 
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded"); 
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    private static readonly int FreeFallHash = Animator.StringToHash("FreeFall");

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        locomotion.OnJumped += HandleJumped;
    }

    private void OnDisable()
    {
        // Unsubscribing prevents errors when this object is destroyed 
        locomotion.OnJumped -= HandleJumped;
    }

    private void Update()
    {
        // Damping lets the blend tree ease between clips instead of snapping 
        _animator.SetFloat(SpeedHash, locomotion.CurrentSpeed, speedDampTime, Time.deltaTime);

        _animator.SetBool(GroundedHash, locomotion.IsGrounded);

        // Only enter the falling state after dropping fast enough 
        bool isFalling = !locomotion.IsGrounded
                         && locomotion.VerticalVelocity < fallThreshold;
        _animator.SetBool(FreeFallHash, isFalling);
    }

    private void HandleJumped()
    {
        _animator.SetTrigger(JumpHash);
    }

    /// <summary> 
    /// Called from an Animation Event placed on the walk and run clips. 
    /// </summary> 
    private void OnFootstep(AnimationEvent animationEvent)
    {
        // Ignore events coming from clips that are barely blended in 
        if (animationEvent.animatorClipInfo.weight <= 0.5f)
        {
            return;
        }
        footstepAudio.PlayRandomFootstep(); 
    }

    /// <summary> 
    /// Called from an Animation Event placed on the landing clip. 
    /// </summary> 
    private void OnLand(AnimationEvent animationEvent)
    { 
        footstepAudio.PlayLanding(); 
    } 
} 
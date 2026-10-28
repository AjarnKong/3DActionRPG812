using UnityEngine;

/// <summary> 
/// Handles the three hit melee combo. 
/// Damage is not applied here, it is triggered by animation events. 
/// </summary> 
[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader input;
    [SerializeField] private DamageDealer weaponDealer;

    [Header("Combo")]
    [SerializeField] private int maxCombo = 3;
    [SerializeField] private float comboResetTime = 1.5f;

    [Header("Damage")]
    [SerializeField] private int baseDamage = 10;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ComboIndexHash = Animator.StringToHash("ComboIndex");

    private Animator _animator;
    private int _comboIndex;
    private float _lastAttackTime;
    private bool _isAttacking;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Waiting too long between presses starts the combo from the beginning 
        if (Time.time - _lastAttackTime > comboResetTime)
        {
            _comboIndex = 0;
        }

        if (input.AttackPressed && !_isAttacking)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        // Cycles through 1, 2, 3 and then back to 1 
        _comboIndex = _comboIndex % maxCombo + 1;

        _animator.SetInteger(ComboIndexHash, _comboIndex);
        _animator.SetTrigger(AttackHash);

        _lastAttackTime = Time.time;
        _isAttacking = true;
    }

    /// <summary> 
    /// Animation event placed where the blade starts to sweep. 
    /// </summary> 
    private void OnAttackWindowOpen()
    {
        weaponDealer.BeginSwing(baseDamage);
    }

    /// <summary> 
    /// Animation event placed where the blade finishes its sweep. 
    /// </summary> 
    private void OnAttackWindowClose()
    {
        weaponDealer.EndSwing();
    }

    /// <summary> 
    /// Animation event placed near the end of the clip. 
    /// Releasing the lock here allows the next combo hit to start. 
    /// </summary> 
    private void OnAttackFinished()
    {
        _isAttacking = false;
    }
}
using UnityEngine;
using UnityEngine.AI;

/// <summary> 
/// Simple finite state machine driving patrol, chase and attack behaviour. 
/// </summary> 
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Chase, Attack, Dead }

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;

    // Larger than detectionRange so the enemy does not flicker at the edge 
    [SerializeField] private float loseTargetRange = 14f;

    [Header("Combat")]
    [SerializeField] private DamageDealer weaponDealer;
    [SerializeField] private int attackDamage = 8;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private NavMeshAgent _agent;
    private Animator _animator;
    private Health _health;
    private Transform _player;

    private State _state = State.Patrol;
    private float _nextAttackTime;
    private int _patrolIndex;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _health = GetComponent<Health>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    private void OnEnable()
    {
        _health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _health.OnDeath -= HandleDeath;
    }

    private void Update()
    {
        if (_state == State.Dead || _player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.position);

        switch (_state)
        {
            case State.Patrol:
                TickPatrol(distance);
                break;
            case State.Chase:
                TickChase(distance);
                break;
            case State.Attack:
                TickAttack(distance);
                break;
        }

        // Feed the real agent speed into the blend tree 
        _animator.SetFloat(SpeedHash, _agent.velocity.magnitude, 0.1f,
            Time.deltaTime);
    }

    private void TickPatrol(float distance)
    {
        if (distance <= detectionRange)
        {
            _state = State.Chase;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        // Move to the next point once the current one is reached (1.8m–2.0m) 
        if (!_agent.pathPending && _agent.remainingDistance < 1.9f)
        {
            _patrolIndex = (_patrolIndex + 1) % patrolPoints.Length;
            _agent.SetDestination(patrolPoints[_patrolIndex].position);
        }
    }

    private void TickChase(float distance)
    {
        if (distance > loseTargetRange)
        {
            _state = State.Patrol;
            return;
        }

        if (distance <= attackRange)
        {
            _state = State.Attack;
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(_player.position);
    }

    private void TickAttack(float distance)
    {
        if (distance > attackRange)
        {
            _state = State.Chase;
            return;
        }

        _agent.isStopped = true;
        FaceTarget();

        if (Time.time >= _nextAttackTime)
        {
            _animator.SetTrigger(AttackHash);
            _nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            return;
        }

        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, target, Time.deltaTime * 8f);
    }

    // Animation events on the enemy attack clip 
    private void OnAttackWindowOpen()
    {
        weaponDealer.BeginSwing(attackDamage);
    }

    private void OnAttackWindowClose()
    {
        weaponDealer.EndSwing();
    }

    private void HandleDeath()
    {
        _state = State.Dead;

        _agent.isStopped = true;
        _animator.SetTrigger(DieHash);

        // Stop the corpse from blocking movement or receiving more hits 
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 4f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
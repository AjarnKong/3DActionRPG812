using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Detects targets during an attack window and applies damage once per swing. 
/// The same component is reused by the player and by every enemy. 
/// </summary> 
public class DamageDealer : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform hitOrigin;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private LayerMask targetLayers;

    // Targets already damaged during the current swing 
    private readonly HashSet<Collider> _alreadyHit = new HashSet<Collider>();

    // Reusing one buffer avoids allocating memory every frame 
    private readonly Collider[] _buffer = new Collider[16];

    private bool _isActive;
    private int _damage;

    public void BeginSwing(int damage)
    {
        _damage = damage;
        _alreadyHit.Clear();
        _isActive = true;
    }

    public void EndSwing()
    {
        _isActive = false;
    }

    private void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }

        int count = Physics.OverlapSphereNonAlloc(
            hitOrigin.position, hitRadius, _buffer, targetLayers);

        for (int i = 0; i < count; i++)
        {
            Collider target = _buffer[i];

            // Add returns false when the target was already hit this swing 
            if (!_alreadyHit.Add(target))
            {
                continue;
            }

            if (target.TryGetComponent(out IDamageable damageable)
                 && !damageable.IsDead)
            {
                damageable.TakeDamage(_damage, hitOrigin.position);
            }
        }
    }

    // Draws the attack sphere in the scene view for easier tuning 
    private void OnDrawGizmosSelected()
    {
        if (hitOrigin == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitOrigin.position, hitRadius);
    }
}
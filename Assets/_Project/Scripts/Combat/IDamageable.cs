using UnityEngine;

/// <summary> 
/// Contract for anything that can receive damage. 
/// Implementing this lets one attack system work on players, enemies andprops.
/// </summary> 
public interface IDamageable
{
    // True once health has reached zero 
    bool IsDead { get; }

    // hitPoint is used to spawn effects at the exact place of impact 
    void TakeDamage(int amount, Vector3 hitPoint);
}

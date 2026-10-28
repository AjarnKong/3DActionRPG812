using System;
using UnityEngine;

/// <summary> 
/// Shared health component used by both the player and every enemy. 
/// It only stores values and announces events, it never reacts on its own. 
/// </summary> 
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject hitEffectPrefab;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    // Sends current and maximum health so a UI bar can be filled directly 
    public event Action<int, int> OnHealthChanged;

    // Sends the impact position so listeners can react at the right place 
    public event Action<Vector3> OnDamaged;

    // Raised once when health reaches zero 
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        // Ignore damage that arrives after death 
        if (IsDead)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnDamaged?.Invoke(hitPoint);

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
        }

        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
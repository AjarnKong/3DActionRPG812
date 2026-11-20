using System;
using UnityEngine;
/// <summary>
/// Tracks the equipped weapon, shows its model and reports total attack power.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private Transform weaponSocket;
    [SerializeField] private int baseAttackPower = 5;
    public WeaponData EquippedWeapon { get; private set; }
    // Read by PlayerCombat every time an attack lands
    public int TotalAttackPower =>
    baseAttackPower + (EquippedWeapon == null ? 0 : EquippedWeapon.attackPower);
    public event Action<WeaponData> OnWeaponChanged;
    private GameObject _weaponInstance;
    public void Equip(WeaponData weapon)
    {
        if (weapon == null)
        {
            return;
        }

        Unequip();
        EquippedWeapon = weapon;
        if (weapon.equipPrefab != null)
        {
            _weaponInstance = Instantiate(weapon.equipPrefab, weaponSocket);
            // Reset the local transform so the model sits in the hand exactly
            _weaponInstance.transform.localPosition = Vector3.zero;
            _weaponInstance.transform.localRotation = Quaternion.identity;
        }
        OnWeaponChanged?.Invoke(weapon);
    }
    public void Unequip()
    {
        if (_weaponInstance != null)
        {
            Destroy(_weaponInstance);
            _weaponInstance = null;
        }
        EquippedWeapon = null;
    }
}
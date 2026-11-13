using UnityEngine;
/// <summary>
/// Weapon specific data. Inherits everything an ordinary item has.
/// </summary>
[CreateAssetMenu(fileName = "NewWeapon", menuName = "RPG/Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Weapon")]
    // Added on top of the character base attack power
    [Min(0)] public int attackPower = 5;
    // Overrides the hit radius of the damage dealer while equipped
    public float hitRadius = 0.6f;
    // Model attached to the hand socket when this weapon is equipped
    public GameObject equipPrefab;
    // Runs in the editor only, keeping weapon assets consistent
    private void OnValidate()
    {
        itemType = ItemType.Weapon;
        maxStackSize = 1;
    }
}
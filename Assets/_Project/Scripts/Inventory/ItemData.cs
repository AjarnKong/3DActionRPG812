using UnityEngine;
public enum ItemType
{
    Weapon,
    Consumable,
    Material
}
/// <summary>
/// Definition data shared by every copy of one item type.
/// Values here never change while the game is running.
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    // Stable text id used when saving and loading the game in week 6
    public string itemId = "item_new";

    public string displayName = "New Item";
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    public ItemType itemType = ItemType.Material;
    [Header("Stacking")]
    // How many copies may share a single inventory slot
    [Min(1)] public int maxStackSize = 10;
    [Header("Economy")]
    // Used by the shop system in week 5
    [Min(0)] public int buyPrice = 10;
    [Min(0)] public int sellPrice = 5;
    [Header("World")]
    // Model spawned when this item is dropped on the ground
    public GameObject worldPrefab;
}
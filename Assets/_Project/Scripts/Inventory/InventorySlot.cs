using System;
/// <summary>
/// One slot in the bag. Plain data, not a component.
/// </summary>
[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;
    public bool IsEmpty => item == null || amount <= 0;
    // True when this slot already holds the same item and has room left
    public bool CanAccept(ItemData other)
    {
        return !IsEmpty && item == other && amount < item.maxStackSize;
    }
    public void Clear()
    {
        item = null;
        amount = 0;
    }
}
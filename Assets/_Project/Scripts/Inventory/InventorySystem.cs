using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Fixed size bag with stacking support.
/// It announces changes through an event so the UI can redraw itself.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int slotCount = 20;

    private readonly List<InventorySlot> _slots = new List<InventorySlot>();
    public IReadOnlyList<InventorySlot> Slots => _slots;
    public event Action OnInventoryChanged;
    private void Awake()
    {
        for (int i = 0; i < slotCount; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }
    /// <summary>
    /// Returns false when the bag could not hold everything.
    /// </summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }
        int remaining = amount;
        // Top up existing stacks first, otherwise the bag fills up too early
        foreach (InventorySlot slot in _slots)
        {
            if (remaining <= 0) break;
            if (!slot.CanAccept(item)) continue;
            int space = item.maxStackSize - slot.amount;
            int moved = Mathf.Min(space, remaining);
            slot.amount += moved;
            remaining -= moved;
        }
        // Then spill the rest into empty slots
        foreach (InventorySlot slot in _slots)
        {
            if (remaining <= 0) break;
            if (!slot.IsEmpty) continue;
            int moved = Mathf.Min(item.maxStackSize, remaining);
            slot.item = item;
            slot.amount = moved;
            remaining -= moved;
        }

        OnInventoryChanged?.Invoke();
        return remaining == 0;
    }
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (CountOf(item) < amount)
        {
            return false;
        }
        int remaining = amount;
        foreach (InventorySlot slot in _slots)
        {
            if (remaining <= 0) break;
            if (slot.IsEmpty || slot.item != item) continue;
            int taken = Mathf.Min(slot.amount, remaining);
            slot.amount -= taken;
            remaining -= taken;
            if (slot.amount <= 0)
            {
                slot.Clear();
            }
        }
        OnInventoryChanged?.Invoke();
        return true;
    }
    public int CountOf(ItemData item)
    {
        int total = 0;
        foreach (InventorySlot slot in _slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                total += slot.amount;
            }
        }
        return total;
    }
    /// <summary>
    /// Checks capacity without changing anything.
    /// The shop in week 5 must call this before taking the player gold.
    /// </summary>
    /// 
    public bool HasSpaceFor(ItemData item, int amount = 1)
    {
        if (item == null) return false;
        int space = 0;
        foreach (InventorySlot slot in _slots)
        {
            if (slot.IsEmpty)
            {
                space += item.maxStackSize;
            }
            else if (slot.item == item)
            {
                space += item.maxStackSize - slot.amount;
            }
            if (space >= amount) return true;
        }
        return false;
    }
}
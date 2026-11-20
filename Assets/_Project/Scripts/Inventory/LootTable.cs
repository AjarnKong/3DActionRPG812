using System;
using UnityEngine;
/// <summary>
/// Weighted random table describing what an enemy may drop.
/// </summary>
[CreateAssetMenu(fileName = "NewLootTable", menuName = "RPG/Loot Table")]
public class LootTable : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public ItemData item;
        [Min(1)] public int minAmount = 1;
        [Min(1)] public int maxAmount = 1;
        // Higher weight means a higher chance relative to the other entries
        [Min(1)] public int weight = 10;
    }
    public Entry[] entries;
    // Chance that anything drops at all
    [Range(0f, 1f)] public float dropChance = 0.7f;
    public Entry Roll()
    {
        if (entries == null || entries.Length == 0)
        {
            return null;
        }

        if (UnityEngine.Random.value > dropChance)
        {
            return null;
        }
        int totalWeight = 0;
        foreach (Entry entry in entries)
        {
            totalWeight += entry.weight;
        }
        int roll = UnityEngine.Random.Range(0, totalWeight);
        // Subtract each weight until the running total goes negative
        foreach (Entry entry in entries)
        {
            roll -= entry.weight;
            if (roll < 0)
            {
                return entry;
            }
        }
        return null;
    }
}
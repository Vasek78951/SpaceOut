using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "NeLootBox", menuName = "Inventory/LootBox")]
public class LootBox : Item
{
    public List<ItemChance> Items;
    public List<Item> GetRandomLoot()
    {
        List<Item> loot = new List<Item>();

        foreach (var itemAmount in Items)
        {
            if (UnityEngine.Random.value <= itemAmount.dropChance)
            {
                int count = UnityEngine.Random.Range(itemAmount.minAmount, itemAmount.maxAmount + 1);
                for (int i = 0; i < count; i++)
                {
                    loot.Add(itemAmount.item);
                }
            }
        }

        return loot;
    }
}
[Serializable]
public class ItemChance
{
    public Item item;
    public int minAmount = 1;
    public int maxAmount = 1;
    [Range(0f, 1f)]
    public float dropChance = 1f;
}
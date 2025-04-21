using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LootAction : ItemAction
{
    private InventoryManager inventoryManager;
    public override void Action(InventoryItem equippedItem)
    {
        if (equippedItem == null) return;

        LootBox lootBox = equippedItem.item as LootBox;
        if (lootBox == null) return;

        if (inventoryManager == null)
            inventoryManager = FindAnyObjectByType<InventoryManager>();

        Debug.Log("loot2");
        List<Item> loot = lootBox.GetRandomLoot();

        foreach (var item in loot)
        {
            inventoryManager.AddItem(item, 1);
        }
        inventoryManager.RemoveItem(equippedItem.item);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public Item item;
    public int amount;

}

[CreateAssetMenu]
public class CraftRecipe : ScriptableObject
{
    public List<ItemAmount> materials;
    public List<ItemAmount> results;


    public bool CanCraft(InventoryManager inventoryManager)
    {

        foreach (ItemAmount itemAmount in materials)
        {
            if (inventoryManager.ItemCount(itemAmount.item) < itemAmount.amount)
            {
                Debug.Log("Cant craft");
                Debug.Log(inventoryManager.ItemCount(itemAmount.item));
                return false;
            }
        }
        Debug.Log("Can craft");
        return true;
    }

    public void CraftItem(InventoryManager inventoryManager)
    {
        Debug.Log("Crafting");
        if (CanCraft(inventoryManager))
        {
            foreach (ItemAmount itemAmount in results)
            {
                for (int i = 0; i < itemAmount.amount; i++)
                {
                    inventoryManager.AddItem(itemAmount.item, 1);
                    Debug.Log("item crafte: " + itemAmount.item);
                }

            }
            foreach (ItemAmount itemAmount in materials)
            {
                for (int i = 0; i < itemAmount.amount; i++)
                {
                    inventoryManager.RemoveItem(itemAmount.item);
                    Debug.Log("removing: " + itemAmount.item);
                }

            }

            
        }
    }

}



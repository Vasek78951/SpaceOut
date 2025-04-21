using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakAction : ItemAction
{
    public Indicator indicator;
    private void Awake()
    {
        indicator = FindAnyObjectByType<Indicator>();
    }
    public override void Action(InventoryItem equippedItem)
    {
        indicator = FindAnyObjectByType<Indicator>();
        if(indicator == null)
            return;
        Debug.Log("Break action");
        GameObject hitObject = indicator.GetObject();
        Debug.Log("hitted object: " + hitObject);
        if (hitObject != null)
        {
            BreakableObject breakable = hitObject.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                float baseDamage = equippedItem.item != null ? equippedItem.item.damage : 1f;
                ItemTypeData itemTypeData = equippedItem.item != null ? equippedItem.item.typeData : null;

                breakable.TakeDamage(baseDamage, itemTypeData);
            }
        }
    }
}

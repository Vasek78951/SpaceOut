using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSlot : InventorySlot
{
    public ItemType allowedItemType;
    public SlotType slotType;
    
    public override void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Item item = dropped.GetComponent<InventoryItem>().item;
        if(item != null && slotType != SlotType.output)
        {
            if(item.type == allowedItemType || allowedItemType == ItemType.None)
            {
                base.OnDrop(eventData);
            }
        }
    }
}
public enum SlotType
{
    input,
    output,
    fuel
}

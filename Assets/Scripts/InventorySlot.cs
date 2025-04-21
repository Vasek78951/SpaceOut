using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        Deselect();
    }
    public void Select()
    {
        image.color = selectedColor;
    }
    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        if (inventoryItem == null) return; // Ensure the dropped object is an InventoryItem

        // Check if this slot already has an item
        if (transform.childCount > 0)
        {
            InventoryItem existingItem = transform.GetComponentInChildren<InventoryItem>();

            // If the dropped item is the same type as the existing item, merge them
            if (inventoryItem.name == existingItem.name)
            {
                existingItem.count += inventoryItem.count; // Add counts together
                existingItem.RefreshCount();
                Destroy(dropped); // Destroy the dropped item
            }
            else
            {
                // If items are different, swap them
                Transform existingItemParent = existingItem.transform.parent;

                // Move the existing item to the dropped item's original slot
                existingItem.transform.SetParent(inventoryItem.parentAfterDrag, false);
                existingItem.transform.localPosition = Vector3.zero;

                // Move the dropped item to this slot
                inventoryItem.transform.SetParent(transform, false);
                inventoryItem.transform.localPosition = Vector3.zero;

                // Update parent references for both items
                inventoryItem.parentAfterDrag = transform;
                existingItem.transform.parent = existingItemParent;
            }
        }

        // Place the dropped item in this slot
        inventoryItem.transform.SetParent(transform, false);
        inventoryItem.transform.localPosition = Vector3.zero;
        inventoryItem.parentAfterDrag = transform;
    }

}

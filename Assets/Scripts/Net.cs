using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxCount = 5;
    private InventoryManager inventoryManager;
    private void Awake()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }
    public void Collect()
    {
        if (inventoryManager != null)
        {
            inventoryManager = FindAnyObjectByType<InventoryManager>();
        }
        foreach (var item in items)
        {
            inventoryManager.AddItem(item, 1);
        }
        items.Clear();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        FloatingItem floatItem = collision.GetComponent<FloatingItem>();
        if (items.Count < maxCount && collision != null && floatItem != null)
        {
            items.Add(floatItem.item);
            Destroy(floatItem.gameObject);
        }
    }
}

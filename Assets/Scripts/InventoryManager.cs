using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class InventoryManager : MonoBehaviour
{
    public int MaxStackItems = 10;
    public InventorySlot[] inventorySlots;
    public InventoryItem inventoryItemPrefab;
    public PlayerHand hand;
    public GameObject InventoryGroup;

    private int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }
    private void Awake()
    {
        hand = FindAnyObjectByType<PlayerHand>();
    }
    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 10)
                ChangeSelectedSlot(number - 1);
        }
    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        if (newValue >= 0 && newValue < inventorySlots.Length)
        {
            inventorySlots[newValue].Select();
            selectedSlot = newValue;
            InventorySlot slot = inventorySlots[newValue];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            hand.EquipItem(itemInSlot);
        }
        else
        {
            Debug.LogError("Invalid slot index.");
        }
    }
    public void AddItemForBtn(Item item)
    {
        AddItem(item, 1);
    }

    public bool AddItem(Item item, int count)
    {
        int countToAdd = count;
        
        if (item.stacable)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null && itemInSlot.item.name == item.name && itemInSlot.count < MaxStackItems)
                {
                    if (itemInSlot.count + countToAdd > MaxStackItems)
                    {
                        
                        int added = MaxStackItems - itemInSlot.count;
                        itemInSlot.count += added;
                        countToAdd -= added;
                        itemInSlot.RefreshCount();
                        AddItem(item, countToAdd);
                        return true;
                    }
                    else
                    {
                        
                        itemInSlot.count += countToAdd;
                        itemInSlot.RefreshCount();
                        return true;
                    }
                    
                }
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                Item newItem = Instantiate(item);
                newItem.count = count;
                newItem.name = item.name;

                SpawnNewItem(newItem, slot);

                if (i == selectedSlot)
                {
                    hand.EquipItem(slot.GetComponentInChildren<InventoryItem>());
                }

                return true;
            }
        }

        return false;
    }


    public bool RemoveItem(Item item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item.name == item.name)
            {
                inventoryItem.count--;
                if (inventoryItem.count <= 0)
                {
                    Destroy(inventoryItem.gameObject);
                }
                else
                {
                    inventoryItem.RefreshCount();
                }
                return true;
            }
        }
        return false;
    }

    public bool ContainsItem(Item item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item.name == item.name)
            {
                return true;
            }
        }
        return false;
    }

    public int ItemCount(Item item)
    {
        Debug.Log(item);
        int count = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item.name == item.name)
            {
                Debug.Log("searching");
                count += inventoryItem.count;
            }
            
        }
        return count;
    }

    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        if (slot == null)
        {
            Debug.LogError("SpawnNewItem: slot is null");
            return;
        }
        Debug.Log(slot);
        InventoryItem inventoryItem = Instantiate(inventoryItemPrefab, slot.transform);
        inventoryItem.InitialiseItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        if (selectedSlot < 0 || selectedSlot >= inventorySlots.Length)
        {
            Debug.LogError("GetSelectedItem: selectedSlot index is out of bounds");
            return null;
        }

        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }
    public bool AddItemToNewSlot(Item item, int count)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                Item newItem = Instantiate(item);
                newItem.count = count;
                newItem.name = item.name;

                SpawnNewItem(newItem, slot);

                if (i == selectedSlot)
                {
                    hand.EquipItem(slot.GetComponentInChildren<InventoryItem>());
                }

                return true;
            }
        }

        Debug.LogWarning("No empty slot found to add item.");
        return false;
    }
    public bool HasItem(Item item, int requiredCount = 1)
    {
        int totalCount = 0;

        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item.name == item.name)
            {
                totalCount += itemInSlot.count;
                if (totalCount >= requiredCount)
                {
                    return true;
                }
            }
        }

        return false;
    }

}

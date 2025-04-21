using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeProccesingObject : MonoBehaviour
{
    public UIMenu uiMenu { get; set; }
    public UIManager uiManager;
    public ObjectWithUI objectWithUI;
    public ItemRecipeDatabse itemRecipeDatabse;
    public InventoryManager inventoryManager;
    public float time = 3;
    private Coroutine processingCoroutine;
    public bool isSmelting = false;
    public float currentTime = 0.0f;
    public float progres = 0;
    public bool needsPower;
    public bool isPowered;
    public float fuel = 0;
    public float fuelUsage;

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        objectWithUI = GetComponent<ObjectWithUI>();
        uiMenu = objectWithUI.uIMenu;
    }
    private void Update()
    {
        if (!isSmelting && CanSmelt())
        {
            StartCoroutine(SmeltItem(time));
        }
    }

    private bool CanSmelt()
    {
        return HasItemsInInput() && HasFuel();
    }
    private bool HasItemsInInput()
    {
        if (objectWithUI.itemsInput == null || objectWithUI.itemsInput.Length == 0)
        {
            Debug.Log("No items in input.");
            return false;
        }

        foreach (var item in objectWithUI.itemsInput)
        {
            if (item != null)
            {
                Debug.Log(item);
                return true;
            }
        }

        Debug.Log("All items have been processed.");
        return false;
    }


    private IEnumerator SmeltItem(float duration)
    {
        if(uiMenu == null)
        {
            uiMenu = objectWithUI.uIMenu;
            Debug.Log("menu: " + uiMenu);
        }
        isSmelting = true;
        Debug.Log("Smelting started!");

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (CanSmelt())
            {
                UseFuel(Time.deltaTime * fuelUsage);
                progres = elapsedTime / duration;
                Debug.LogWarning(uiMenu);
                uiMenu.UpdateUI();
                elapsedTime += Time.deltaTime;
                Debug.Log("can smelt");
                yield return null;
            }
            else
            {
                progres = 0;
                elapsedTime = 0f;
                uiMenu.UpdateUI();
                yield return null;
            }
        }

        if (HasItemsInInput())
        {
            CraftItem();
        }

        progres = 0;
        uiMenu.UpdateUI();
        isSmelting = false;
    }


    private void UseFuel(float amount)
    {
        if (!needsPower) return;

        if (fuel <= 0f)
        {
            fuel = GetFuel();
            if (fuel <= 0f) return;
        }

        fuel -= amount;
        if (fuel < 0f) fuel = 0f;

        Debug.Log($"Fuel remaining: {fuel:F2}");
    }
    private float GetFuel()
    {
        for (int i = 0; i < objectWithUI.itemsFuel.Length; i++)
        {
            var fuelItem = objectWithUI.itemsFuel[i];
            if (fuelItem != null && fuelItem.item.category == ItemCategory.Fuel)
            {
                float burnTime = fuelItem.item.consumeTime;
                fuelItem.RemoveItem();

                if (uiMenu != null)
                {
                    uiManager.currentObjectUI.UpdateUI();
                }

                return burnTime;
            }
        }

        return 0f;
    }
    private bool HasFuel()
    {
        if (fuel > 0f || !needsPower) return true;

        foreach (var fuelItem in objectWithUI.itemsFuel)
        {
            
            Debug.Log(fuelItem);
            if (fuelItem != null && fuelItem.item.category == ItemCategory.Fuel)
            {
                Debug.Log("hasfuel");
                return true;
            }
        }
        Debug.Log("doesnt have fuel");
        return false;
    }
    private void RemoveItem(InventoryItem item, int index)
    {
        Debug.Log("removing item");
        if (item == null)
        {
            Debug.LogWarning($"RemoveItem called with a null item at index {index}.");
            return;
        }

        item.count--; // Decrease the item count
        if (item.count <= 0) // If the count is zero or less, remove the item
        {
            Debug.Log($"Removing item '{item.item.name}' from index {index}.");
            objectWithUI.itemsInput[index] = null; // Remove the item from the input array
            Destroy(item.gameObject); // Destroy the item's GameObject
        }
        else
        {
            Debug.Log($"Decreasing item count for '{item.item.name}' to {item.count}.");
            item.RefreshCount(); // Update the UI display for the item count
        }

        // Force a UI refresh to reflect the updated state
        if (uiMenu != null)
        {
            uiMenu.UpdateUI();
        }
    }


    private void CraftItem()
    {
        List<(InventoryItem, int)> itemsToRemove = new List<(InventoryItem, int)>();

        // Loop through all input slots
        for (int i = 0; i < objectWithUI.itemsInput.Length; i++)
        {
            InventoryItem inputItem = objectWithUI.itemsInput[i];
            if (inputItem == null) continue; // Skip null slots

            foreach (Recipe recipe in itemRecipeDatabse.itemRecipeDatabse)
            {
                bool matchFound = true;

                // Check if the recipe's input items match
                foreach (ItemRequirement inputRequirement in recipe.inputItems)
                {
                    if (inputItem.item.name != inputRequirement.item.name || inputItem.count < inputRequirement.count)
                    {
                        matchFound = false;
                        break;
                    }
                }

                if (matchFound)
                {
                    // Collect items to remove
                    foreach (ItemRequirement inputRequirement in recipe.inputItems)
                    {
                        for (int k = 0; k < objectWithUI.itemsInput.Length; k++)
                        {
                            InventoryItem slotItem = objectWithUI.itemsInput[k];
                            if (slotItem != null && slotItem.item.name == inputRequirement.item.name)
                            {
                                itemsToRemove.Add((slotItem, k)); // Track the item and its index
                            }
                        }
                    }

                    // Add crafted output items
                    foreach (ItemRequirement outputRequirement in recipe.outputItems)
                    {
                        for (int k = 0; k < outputRequirement.count; k++)
                        {
                            Debug.LogWarning(outputRequirement.item);
                            InventoryItem craftedItem = AddItem(outputRequirement.item);

                            if (craftedItem != null && objectWithUI.itemsOutput.Length > 0)
                            {
                                for (int outputIndex = 0; outputIndex < objectWithUI.itemsOutput.Length; outputIndex++)
                                {
                                    if (objectWithUI.itemsOutput[outputIndex] == null)
                                    {
                                        objectWithUI.itemsOutput[outputIndex] = craftedItem;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Update UI after crafting
                    if (uiMenu != null)
                    {
                        uiManager.currentObjectUI.UpdateUI();
                    }

                    break; // Exit after processing the first valid recipe
                }
            }
        }
        // Process item removals after crafting
        foreach (var (item, index) in itemsToRemove)
        {
            RemoveItem(item, index);
        }
    }

    public InventoryItem AddItem(Item item)
    {
        if (item.stacable)
        {
            foreach (ObjectSlot slot in objectWithUI.uIMenu.outputSlots)
            {
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null && itemInSlot.item.name == item.name && itemInSlot.count < item.maxStack)
                {
                    itemInSlot.count++;
                    itemInSlot.RefreshCount();
                    return itemInSlot;
                }
            }
        }

        foreach (ObjectSlot slot in objectWithUI.uIMenu.outputSlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                Debug.Log("item spawned");
                inventoryManager.SpawnNewItem(item, slot);
                return slot.GetComponentInChildren<InventoryItem>();
            }
        }

        return null;
    }
}
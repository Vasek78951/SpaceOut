using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratingObject : MonoBehaviour
{
    public UIMenu uIMenu { get; set; }
    public UIManager uiManager;
    public ObjectWithUI objectWithUI;
    public ItemRecipeDatabse itemRecipeDatabse;
    public InventoryManager inventoryManager;
    public float time = 3;
    private Coroutine processingCoroutine;
    public bool isGenerating = false;
    public float currentTime = 0.0f;
    public float progres = 0;
    public bool needsPower;
    public float fuel = 0;
    public float fuelUsage;
    public float generateSpeed = 1;
    public float storage = 0;
    public CableManager cableManager;

    private void Start()
    {

    }

    private void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        objectWithUI = GetComponent<ObjectWithUI>();
        uIMenu = objectWithUI.uIMenu;
        cableManager = FindAnyObjectByType<CableManager>();
        
       
    }
    private void Update()
    {
        cableManager.PowerAround(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        Debug.Log(transform.position);
        //if (isGenerating)
        //{
        //    Debug.Log("Smelting is already in progress.");
        //    return;
        //}

        //if (!HasFuel())
        //{
        //    Debug.Log("doesnt have fuel");
        //    return;
        //}
        //StartCoroutine(Generate(time));
    }

    private IEnumerator Generate(float duration)
    {
        Debug.Log("generating");
        while(HasFuel())
        {
            storage += generateSpeed * Time.deltaTime;
            UseFuel(Time.deltaTime * fuelUsage);
            isGenerating = true;
            uIMenu.UpdateGenerationAmount();
        }
        isGenerating = false;
        yield return null;
        //Debug.Log("Smelting started!");

        //float elapsedTime = 0f;

        //while (elapsedTime < duration)
        //{
        //    if (HasFuel())
        //    {
        //        UseFuel(Time.deltaTime * fuelUsage);
        //        progres = elapsedTime / duration;
        //        uIMenu.UpdateUI();
        //        elapsedTime += Time.deltaTime;
        //        yield return null;
        //    }
        //    else
        //    {
        //        progres = 0;
        //        elapsedTime = 0f;
        //        uIMenu.UpdateUI();
        //        yield return null;
        //    }
        //}

        //if (HasItemsInInput())
        //{
        //    //CraftItem();
        //}

        //progres = 0;
        //uIMenu.UpdateUI();

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

                if (uIMenu != null)
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
            if (fuelItem != null && fuelItem.item.category == ItemCategory.Fuel)
            {
                Debug.Log("has coal");
                return true;
            }
        }
        Debug.Log("doesnt have coal");
        return false;
    }
    private void RemoveItem(InventoryItem item, int index)
    {
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
        if (uIMenu != null)
        {
            uIMenu.UpdateUI();
        }
    }
}

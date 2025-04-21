using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CustomInput input;
    public GameObject inventory;
    public GameObject dropSlots;
    public GameObject background;
    public UIMenu currentObjectUI;
    private ObjectWithUI currentObject;
    public Canvas playerCanvas;
    public bool dontLoad;

    private void Awake()
    {
        input = new CustomInput();
        dontLoad = false;
    }

    private void Update()
    {
        /*
        if (currentObject != null && currentObject.uIMenu != null && dontLoad)
        {
            var slots = currentObject.uIMenu.GetComponentsInChildren<ObjectSlot>();
            InventoryItem[] items = new InventoryItem[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                items[i] = slots[i].GetComponentInChildren<InventoryItem>();
            }
            currentObject.itemsInput = items;
            foreach (InventoryItem item in currentObject.itemsInput)
            {
                if (item == null) return;
            }
        }*/
    }
    public void OpenObjectUI(ObjectWithUI objectWithUI)
    {
        if (currentObjectUI != null)
        {
            Debug.LogWarning("A UI is already open. Close it before opening another.");
            return; // comment this line if you prefer auto-close
                    // CloseObjectUI(); // uncomment this line if you want to replace the current UI automatically
        }

        dontLoad = true;
        currentObject = objectWithUI;
        UIMenu UI = objectWithUI.uiPrefab;
        if (UI == null || inventory == null) return;

        currentObjectUI = Instantiate(UI, playerCanvas.transform);
        currentObjectUI.Initialize(objectWithUI);
        currentObjectUI.objectWithUI = objectWithUI;

        Debug.Log("object: " + objectWithUI);

        inventory.SetActive(true);
        dropSlots.SetActive(true);
        inventory.GetComponent<RectTransform>().anchoredPosition = new Vector2(-450, 0);
        currentObjectUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(475, 0);

        currentObjectUI.UpdateUI();
    }

    public void OpenObjectUIWithoutInv(ObjectWithUI objectWithUI)
    {
        if (currentObjectUI != null)
        {
            Debug.LogWarning("A UI is already open. Close it before opening another.");
            return;
        }

        currentObject = objectWithUI;
        UIMenu UI = objectWithUI.uiPrefab;
        if (UI == null || inventory == null) return;

        currentObjectUI = Instantiate(UI, playerCanvas.transform);
        objectWithUI.uIMenu = currentObjectUI;
        currentObjectUI.objectWithUI = objectWithUI;

        Debug.Log("object: " + objectWithUI);

        dropSlots.SetActive(true);
        currentObjectUI.UpdateUI();
    }


    public void CloseObjectUI()
    {
        currentObjectUI.canUpdate = false;
        dontLoad = false;
        if (currentObjectUI == null || currentObject == null || currentObject.itemsInput == null) return;
        Debug.Log("closing");
        var objectSlots = currentObjectUI.GetComponentsInChildren<ObjectSlot>();
        if (objectSlots != null && objectSlots.Length != 0)
        {
            for (int i = 0; i < objectSlots.Length; i++)
            {
                
                var item = objectSlots[i].GetComponentInChildren<InventoryItem>();
                if (item != null)
                {
                    Debug.Log("saving");
                    item.transform.SetParent(currentObject.transform, false);
                }
            }
        }
        Destroy(currentObjectUI.gameObject);
    }

    public void UpdateUI()
    {
        
        if (currentObject == null || currentObjectUI == null) return;

        // Update the progress bar
        

        // Find all object slots in the UI menu
        var objectSlots = currentObjectUI.GetComponentsInChildren<ObjectSlot>();
        
        if (objectSlots.Length == 0) return;
        
        // Iterate over each slot and update or clear it
        for (int i = 0; i < currentObject.itemsInput.Length; i++)
        {
            if (currentObject.itemsInput[i] != null && i < objectSlots.Length)
            {
                var objectSlot = objectSlots[i];
                InventoryItem item = currentObject.itemsInput[i];

                // If the item is not already in the correct slot, update the slot
                if (item.transform.parent != objectSlot.transform)
                {
                    item.transform.SetParent(objectSlot.transform, false);
                }
            }
            else if (i < objectSlots.Length)
            {
                // If no item in the slot, clear the UI slot
                var objectSlot = objectSlots[i];
                var item = objectSlot.GetComponentInChildren<InventoryItem>();
                if (item != null)
                {
                    Destroy(item.gameObject); // Destroy the item in the slot
                }
            }
        }
    }


    private void Inventory(InputAction.CallbackContext context)
    {
        if (inventory == null) return;

        // Toggle inventory visibility
        inventory.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        if (currentObjectUI != null)
        {
            Debug.Log("Closing menu");
            CloseObjectUI();
        }

        if (inventory.activeSelf || currentObjectUI != null)
        {
            inventory.SetActive(false);
            dropSlots.SetActive(false);
        }
        else
        {
            inventory.SetActive(true);
            dropSlots.SetActive(true);
        }
    }

    private void OnEnable()
    {
        input.Enable();
        input.UI.OpenInventory.performed += Inventory;
    }

    private void OnDisable()
    {
        input.UI.OpenInventory.performed -= Inventory;
        input.Disable();
    }
}

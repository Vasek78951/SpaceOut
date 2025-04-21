using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public ObjectWithUI objectWithUI;
    public Slider progressBar;
    public Slider fuelIndicator;
    public ObjectSlot[] inputSlots;
    public ObjectSlot[] outputSlots;
    public ObjectSlot[] fuelSlots;
    public TextMeshProUGUI generatingStorage;
    ObjectSlot[][] objSlots;
    private bool isUIUpdated;
    public bool canUpdate;

    private void Start()
    {
        Debug.LogWarning(objectWithUI);
        objectWithUI.GetComponent<ObjectWithUI>().uIMenu = this;

        LoadSlots();
    }
    public void Initialize(ObjectWithUI obj)
    {
        objectWithUI = obj;
        objectWithUI.uIMenu = this;
        isUIUpdated = false;
        canUpdate = true;
        LoadSlots();
        UpdateUI();
    }
    private void LoadSlots()
    {
        SlotType[] slotTypes = { SlotType.input, SlotType.output, SlotType.fuel };
        objSlots = new ObjectSlot[slotTypes.Length][];

        List<ObjectSlot>[] slotLists = new List<ObjectSlot>[slotTypes.Length];
        for (int i = 0; i < slotLists.Length; i++)
        {
            slotLists[i] = new List<ObjectSlot>();
        }

        var allSlots = GetComponentsInChildren<ObjectSlot>();

        foreach (var slot in allSlots)
        {
            for (int i = 0; i < slotTypes.Length; i++)
            {
                if (slot.slotType == slotTypes[i])
                {
                    slotLists[i].Add(slot);
                    break;
                }
            }
        }

        for (int i = 0; i < slotLists.Length; i++)
        {
            objSlots[i] = slotLists[i].ToArray();
        }

        inputSlots = objSlots[0];
        outputSlots = objSlots[1];
        fuelSlots = objSlots[2];
    }


    private void Update()
    {
        if (objectWithUI == null || objectWithUI.uIMenu != this || !isUIUpdated) return;

        var slots = objectWithUI.uIMenu.GetComponentsInChildren<ObjectSlot>();
        Debug.Log("updating2");
        List<InventoryItem> inputItems = new List<InventoryItem>();
        List<InventoryItem> outputItems = new List<InventoryItem>();
        List<InventoryItem> fuelItems = new List<InventoryItem>();

        foreach (var slot in slots)
        {
            var item = slot.GetComponentInChildren<InventoryItem>();
            switch (slot.slotType)
            {
                case SlotType.input:
                    inputItems.Add(item);
                    break;
                case SlotType.output:
                    outputItems.Add(item);
                    break;
                case SlotType.fuel:
                    fuelItems.Add(item);
                    break;
            }
        }

        objectWithUI.itemsInput = inputItems.ToArray();
        objectWithUI.itemsOutput = outputItems.ToArray();
        objectWithUI.itemsFuel = fuelItems.ToArray();
        objectWithUI.objItems = new InventoryItem[][]
        {
            objectWithUI.itemsInput,
            objectWithUI.itemsOutput,
            objectWithUI.itemsFuel
        };
    }
    public void UpdateGenerationAmount()
    {
        generatingStorage.text = objectWithUI.GetComponent<GeneratingObject>().storage.ToString();
    }
    public void UpdateProgresBar()
    {
        progressBar.value = objectWithUI.GetComponent<TimeProccesingObject>().progres;
    }
    public void UpdateFuelIndicator()
    {
        fuelIndicator.value = objectWithUI.GetComponent<TimeProccesingObject>().fuel;
    }

    public void UpdateUI()
    {
        if (objectWithUI == null) return;
        
        TimeProccesingObject timeProcObj = objectWithUI.GetComponent<TimeProccesingObject>();
        if (timeProcObj != null)
        {
            UpdateProgresBar();
            UpdateFuelIndicator();
        }

        for (int i = 0; i < objSlots.Length; i++)
        {
            if (objSlots[i] == null) continue;

            for (int j = 0; j < objSlots[i].Length; j++)
            {
                if (objSlots[i][j] == null) continue;

                ObjectSlot objectSlot = objSlots[i][j];
                if (objectWithUI.objItems[i] != null && j < objectWithUI.objItems[i].Length)
                {
                    InventoryItem item = objectWithUI.objItems[i][j];
                    Debug.Log("item:" + item);
                    if (item != null)
                    {
                        if (item.transform.parent != objectSlot.transform)
                        {
                            item.transform.SetParent(objectSlot.transform, false);
                        }
                    }
                    else
                    {
                        InventoryItem existingItem = objectSlot.GetComponentInChildren<InventoryItem>();
                        if (existingItem != null)
                        {
                            Destroy(existingItem.gameObject);
                        }
                    }
                }
            }
        }
        isUIUpdated = true;
    }
}

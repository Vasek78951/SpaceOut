using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWithUI : MonoBehaviour, PlacableObject, InteractiveObject
{
    public UIMenu uiPrefab;
    public UIMenu uIMenu;
    public UIManager uiManager;
    public InventoryItem[] itemsInput;
    public InventoryItem[] itemsOutput;
    public InventoryItem[] itemsFuel;
    public InventoryItem[][] objItems;
    public bool needsInv = false;
    [SerializeField] public List<ObjectSlot> slots = new List<ObjectSlot>();

    private void Start()
    {
        if (slots == null || slots.Count == 0)
        {
            Debug.LogWarning("Slots list is null or empty!");
            return;
        }

        int inputCount = slots.FindAll(slot => slot != null && slot.slotType == SlotType.input).Count;
        int outputCount = slots.FindAll(slot => slot != null && slot.slotType == SlotType.output).Count;
        int fuelCount = slots.FindAll(slot => slot != null && slot.slotType == SlotType.fuel).Count;

        itemsInput = new InventoryItem[inputCount];
        itemsOutput = new InventoryItem[outputCount];
        itemsFuel = new InventoryItem[fuelCount];

        objItems = new InventoryItem[][] { itemsInput, itemsOutput, itemsFuel };
    }


    public void Interaction()
    {
        Debug.Log("Interacting");
        if(needsInv)
        {
            uiManager.OpenObjectUI(this);
        }
        else
        {
            uiManager.OpenObjectUIWithoutInv(this);
        }
        
    }

    public void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        objItems = new InventoryItem[][] { itemsInput, itemsOutput, itemsFuel };
    }

    
}

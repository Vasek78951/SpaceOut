using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour
{
    public CraftRecipe craftRecipe;
    public Image icon;
    private InventoryManager inventoryManager;

    public void Start()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        icon.sprite = craftRecipe.results[0].item.icon;
    }
    public void Craft()
    {
        Debug.Log("Craft");
        craftRecipe.CraftItem(inventoryManager);
    }
    public void OnClick()
    {
        Debug.Log("click");
        CraftingDetailPanel.Instance.ShowDetails(craftRecipe);
    }
}

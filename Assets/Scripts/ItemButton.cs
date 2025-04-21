using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public Item assignedItem;  // Assign the ScriptableObject in Inspector

    public void OnClick()
    {
        FindObjectOfType<InventoryManager>().AddItemForBtn(assignedItem);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemAction : MonoBehaviour
{
    public abstract void Action(InventoryItem equippedItem);
}

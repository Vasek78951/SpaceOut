using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerHand : MonoBehaviour
{
    public Animator playerAnimator;
    public Animator itemAnimator;
    public Transform handTransform;
    private InventoryItem equippedItemObj;
    private Item equippedItem;
    public Item hand;

    public void Awake()
    {

    }
    public void EquipItem(InventoryItem invItem)
    {
        if (invItem != null)
        {
            if (invItem.item.name != equippedItem.name) 
            {
                equippedItem = invItem.item;
            }
        }
        else
        {
            if (hand != equippedItem)
                equippedItem = hand;
        }
        if(equippedItemObj != null)
        {
            Destroy(equippedItemObj.gameObject);
        }
        
        if (equippedItem != null && equippedItem.worldPrefab != null)
        {
            equippedItemObj = Instantiate(equippedItem.worldPrefab, handTransform);
            equippedItemObj.item = equippedItem;
            itemAnimator = equippedItemObj.GetComponent<Animator>();

            if (equippedItem.itemAnimator != null)
            {
                itemAnimator.runtimeAnimatorController = equippedItem.itemAnimator.runtimeAnimatorController;
            }
        }
    }

    public void UseItem()
    {
        if (equippedItemObj != null)
        {
            Item itemData = equippedItemObj.item;
            if (itemData != null)
            {
                //itemData.Use(playerAnimator, itemAnimator);
            }
        }
    }
    public void LeftClick()
    {
        Debug.Log("left click");
        if(equippedItemObj != null)
        {
            equippedItem.leftClick.Action(equippedItemObj);
        }
    }
    public void RightClick()
    {
        Debug.Log("right click");
        equippedItem.rightClick.Action(equippedItemObj);
    }
    public void ItemAction(ItemAction action, Item item)
    {

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click to use item
        {
            UseItem();
            LeftClick();
        }

        if (Input.GetMouseButtonDown(1)) // Right click to use item
        {
            UseItem();
            RightClick();
        }
    }
}

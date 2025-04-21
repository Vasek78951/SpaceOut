using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DropSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public dropTo type;
    public GameObject droppedItemPref;
    public GameObject floatingItemsPref;
    public Transform playerT;
    private UnityEngine.UI.Image image;
    private Color originalColor;
    public Color hoverColor;

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem invItem = eventData.pointerDrag.gameObject.GetComponent<InventoryItem>();
        if (invItem == null)
            return;
        if (type == dropTo.ground)
        {
            GameObject droppedItemObject = Instantiate(droppedItemPref, playerT.position, Quaternion.identity);

            // Assign item properties to the dropped item
            DroppedItem droppedItem = droppedItemObject.GetComponent<DroppedItem>();
            if (droppedItem != null)
            {
                droppedItem.InitialiseItem(invItem.item);
            }
            else
            {
                Debug.LogError("DroppedItem script not found on droppedItemPref!");
            }

            // Reset this item's count and destroy it from the inventory
            Destroy(invItem.gameObject);
        }
        if (type == dropTo.air)
        {
            GameObject floatingItemObject = Instantiate(floatingItemsPref, playerT.position, Quaternion.identity);

            // Assign item properties to the dropped item
            FloatingItem floatingItem = floatingItemObject.GetComponent<FloatingItem>();
            if (floatingItem != null)
            {
                floatingItem.InitialiseItem(invItem.item);
            }
            else
            {
                Debug.LogError("FloatingItem script not found on floatinhItemsPref!");
            }

            // Reset this item's count and destroy it from the inventory
            Destroy(invItem.gameObject);
        }
        ResetColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;
        InventoryItem item = eventData.pointerDrag.gameObject.GetComponent<InventoryItem>();
        if (item != null)
        {
            Debug.Log("ON");
            if (image != null)
            {
                image.color = hoverColor;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetColor();
    }
    private void ResetColor()
    {
        if (image != null)
        {
            image.color = originalColor;
        }
    }
    private void Awake()
    {
        playerT = FindAnyObjectByType<PlayerMovement>().transform;
        image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            originalColor = image.color;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public enum dropTo
    {
        ground,
        air
        
    }
}

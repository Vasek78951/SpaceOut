using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public GameObject InventoryItemPref;
    public GameObject DropItemPref;
    public GameObject DropFloatItemPref;
    public Item item;
    public TextMeshProUGUI countText;
    public Image image;
    public int count = 1;
    public Transform parentAfterDrag;
    public RuntimeAnimatorController animatorController;
    //public ItemType type;
    //public ActionType actionType;
    public float damage;
    Raft raft;
    Vector2 raftPostion;
    Transform playerT;
    public GameObject splitPopupPrefab;
    public Transform canvasTransform;
    public ulong PlayerId { get; private set; }
     

    public void Initialize(ulong playerId)
    {
        PlayerId = playerId;
        // Additional initialization code if needed
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        raft = FindAnyObjectByType<Raft>();
        playerT = FindAnyObjectByType<PlayerMovement>().transform;
        canvasTransform = GameObject.FindGameObjectWithTag("PlayerCanvas").transform;
    }

    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.icon;
        count = item.count;
        //animatorController = newItem.animatorController;
        //type = newItem.type;
        //actionType = newItem.actionType;
        //damage = newItem.damage;
        RefreshCount();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("clicking right mouse");
            ShowSplitPopup();
        }
    }

    void ShowSplitPopup()
    {
        GameObject popup = Instantiate(splitPopupPrefab, canvasTransform);
        popup.transform.position = Input.mousePosition;

        InvetoryPopup popupScript = popup.GetComponent<InvetoryPopup>();
        popupScript.Setup(this); // Pass reference to this item
    }
    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textVisible = count > 1;
        countText.gameObject.SetActive(textVisible);
    }
    public void RemoveItem()
    {
        count--;
        if (count <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            RefreshCount();
        }
    }
    public void DropItem()
    {
        if (count <= 0 || DropItemPref == null) return;

        // Instantiate the dropped item in the world
        GameObject droppedItemObject = Instantiate(DropItemPref, playerT.position, Quaternion.identity);

        // Assign item properties to the dropped item
        DroppedItem droppedItem = droppedItemObject.GetComponent<DroppedItem>();
        if (droppedItem != null)
        {
            droppedItem.item = item;
            droppedItem.count = count;
            droppedItem.canPickUp = false;
        }
        else
        {
            Debug.LogError("DroppedItem script not found on DropItemPref!");
        }

        // Reset this item's count and destroy it from the inventory
        count = 0;
        RefreshCount();
        Destroy(gameObject);

    }
    public void DropFloatItem()
    {
        if (count <= 0 || DropItemPref == null) return;

        // Instantiate the dropped item in the world
        GameObject droppedItemObject = Instantiate(DropFloatItemPref, playerT.position, Quaternion.identity);

        // Assign item properties to the dropped item
        FloatingItem floatingItem = droppedItemObject.GetComponent<FloatingItem>();
        if (floatingItem != null)
        {
            floatingItem.item = item;
        }
        else
        {
            Debug.LogError("DroppedItem script not found on DropItemPref!");
        }

        // Reset this item's count and destroy it from the inventory
        count = 0;
        RefreshCount();
        Destroy(gameObject);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        raftPostion = raft.transform.position;
        Canvas canvas = GetComponentInParent<Canvas>();
        transform.SetParent(canvas.transform, false);
        transform.localScale = Vector3.one;
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Transform originalParent = parentAfterDrag;
        //if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InventorySlot>() == null)
        //{
        //    DropItem();
        //    return;
        //}
        transform.SetParent(parentAfterDrag, false);
        transform.localScale = Vector3.one;
        image.raycastTarget = true;
    }
}

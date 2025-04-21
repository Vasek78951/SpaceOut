using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvetoryPopup : MonoBehaviour
{
    public static InvetoryPopup currentPopup;

    public TMP_InputField inputField;
    public Button splitButton;
    public Button cancelButton;

    private InventoryItem sourceItem;

    public void Setup(InventoryItem item)
    {
        // Close the previous one if it's still open
        if (currentPopup != null && currentPopup != this)
        {
            Destroy(currentPopup.gameObject);
        }

        currentPopup = this;

        sourceItem = item;
        //inputField.text = (item.count / 2).ToString();

        splitButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        splitButton.onClick.AddListener(OnSplit);
        cancelButton.onClick.AddListener(() => Destroy(gameObject));
    }

    void OnDestroy()
    {
        if (currentPopup == this)
            currentPopup = null;
    }

    void OnSplit()
    {
        int splitAmount = sourceItem.count / 2;

        if (splitAmount <= 0 || splitAmount >= sourceItem.count) return;

        sourceItem.count -= splitAmount;
        sourceItem.RefreshCount();

        InventoryManager invManager = FindObjectOfType<InventoryManager>();

        Item splitItem = Instantiate(sourceItem.item);
        splitItem.count = splitAmount;

        invManager.AddItemToNewSlot(splitItem, splitAmount);

        Destroy(gameObject);
    }
}

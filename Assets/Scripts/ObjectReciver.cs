using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveReceiver : MonoBehaviour
{
    public RequiredItem[] requiredItems; // Assign the required items in order via Inspector
    public InventoryManager inventoryManager; // Reference to the player's inventory
    private int currentIndex = 0;
    public Image itemIcon;
    public TextMeshProUGUI amount;
    public GameObject victoryScreen; // Optional UI for end game

    private void Start()
    {
        if (requiredItems.Length == 0)
        {
            Debug.LogWarning("No items set for ObjectiveReceiver.");
        }

        if (inventoryManager == null)
        {
            inventoryManager = FindAnyObjectByType<InventoryManager>();
        }
        itemIcon.sprite = requiredItems[currentIndex].item.icon;
        amount.text = requiredItems[currentIndex].amount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogWarning("complete");
        if (currentIndex >= requiredItems.Length)
            return;

        // Make sure the player has collided
        if (other.CompareTag("Player"))
        {
            Item currentRequiredItem = requiredItems[currentIndex].item;
            if (inventoryManager.HasItem(currentRequiredItem, requiredItems[currentIndex].amount))
            {
                // Consume one of the required item
                for (int i = 0; i < requiredItems[currentIndex].amount; i++)
                {
                    inventoryManager.RemoveItem(currentRequiredItem);
                }
                Debug.Log($"Delivered item {currentIndex + 1}/{requiredItems.Length}: {currentRequiredItem.name}");
                currentIndex++;

                if (currentIndex >= requiredItems.Length)
                {
                    CompleteObjective();
                }
                else
                {
                    itemIcon.sprite = requiredItems[currentIndex].item.icon;
                    amount.text = requiredItems[currentIndex].amount.ToString();
                }

            }
            else
            {
                Debug.Log($"You need to deliver: {currentRequiredItem.name}");
            }
        }
    }

    private void CompleteObjective()
    {
        Debug.Log("All items delivered! Objective complete.");
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }
        // You can also call SceneManager.LoadScene("WinScene") or pause the game
        // Time.timeScale = 0f;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingDetailPanel : MonoBehaviour
{
    public static CraftingDetailPanel Instance;

    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    private CraftRecipe currentRecipe;
    public Transform materialListParent;
    public GameObject materialEntryPrefab;
    public Button craftButton;
    private void Awake()
    {
        Instance = this; 
        gameObject.SetActive(false);
    }
    public void ShowDetails(CraftRecipe recipe)
    {
        Debug.Log("showing menu");
        currentRecipe = recipe;
        gameObject.SetActive(true);

        var result = recipe.results[0];
        itemIcon.sprite = result.item.icon;
        itemName.text = result.item.itemName;
        //description.text = result.item.description;

        foreach (Transform child in materialListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var material in recipe.materials)
        {
            GameObject entry = Instantiate(materialEntryPrefab, materialListParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = $"{material.amount}x {material.item.itemName}";
            entry.GetComponentInChildren<Image>().sprite = material.item.icon;
        }

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() =>
        {
            Debug.Log("crafting");
            recipe.CraftItem(FindAnyObjectByType<InventoryManager>());
            ShowDetails(recipe); // Refresh
        });
        Debug.Log("sDone");
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

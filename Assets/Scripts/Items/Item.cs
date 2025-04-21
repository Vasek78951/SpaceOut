using Unity.Burst.CompilerServices;
using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public InventoryItem worldPrefab;
    public bool stacable;
    public int maxStack;
    public int count;
    public ItemType type;
    public ItemCategory category;
    public float damage;
    public Animator itemAnimator;
    public ItemTypeData typeData;
    public float consumeTime;
    public ItemAction leftClick;
    public ItemAction rightClick;

    public void Use(Animator playerAnimator, Animator itemAnimator)
    {
        playerAnimator.SetTrigger("Attack"); // Play player attack animation
        if (itemAnimator != null)
        {
            itemAnimator.SetTrigger("Swing"); // Play weapon animation
        }
    }
}

[CreateAssetMenu(fileName = "NewItemType", menuName = "Item Type Data")]
public class ItemTypeData : ScriptableObject
{
    public ItemType type;
    public float baseDamage;
    public float stoneMultiplier = 1f;
    public float woodMultiplier = 1f;
    public float metalMultiplier = 1f;

    public float GetDamageMultiplier(ObjectType objectType)
    {
        switch (objectType)
        {
            case ObjectType.Rock: return stoneMultiplier;
            case ObjectType.Wood: return woodMultiplier;
            case ObjectType.Metal: return metalMultiplier;
            default: return 1f;
        }
    }
}

public enum ItemCategory
{
    Weapon,
    Tool,
    Resource,
    Fuel
}

public enum ItemType
{
    Sword,
    Axe,
    Bow,
    Hammer,
    Pickaxe,
    Shovel,
    None
}
public enum ObjectType { Generic, Rock, Wood, Metal }
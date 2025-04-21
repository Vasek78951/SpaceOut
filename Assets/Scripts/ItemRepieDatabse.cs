using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemRecipeDatabse : ScriptableObject
{
    public List<Recipe> itemRecipeDatabse;
}

[Serializable]
public class Recipe
{
    [field: SerializeField]
    public List<ItemRequirement> inputItems = new List<ItemRequirement>();

    [field: SerializeField]
    public List<ItemRequirement> outputItems = new List<ItemRequirement>();
}

[Serializable]
public class ItemRequirement
{
    [field: SerializeField]
    public Item item { get; private set; }

    [field: SerializeField]
    public int count { get; private set; }
}

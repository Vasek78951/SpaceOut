using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class buildingObjectDatabase : ScriptableObject
{
    public List<ObjectData> objectsData;
}
[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public BuildingObject Object { get; private set; }
    [field: SerializeField] public List<RequiredItem> requiredItems { get; private set; }

}

[Serializable]
public class RequiredItem
{
    public Item item;
    public int amount;
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlacementObjectDatabase : ScriptableObject
{
    public List<PlacementObjectData> palcementObjectsData;
}

[Serializable]
public class PlacementObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public Vector2 Size { get; private set; } = Vector2.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}

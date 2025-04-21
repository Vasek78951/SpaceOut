using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCollector : MonoBehaviour, InteractiveObject
{
    public Net net;
    public void Interaction()
    {
        net.Collect();
    }
}

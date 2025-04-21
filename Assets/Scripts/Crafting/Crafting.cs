using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour, InteractiveObject
{
    public GameObject UIPrefab;
    private UIManager uiManager;
    public void Awake()
    {
        uiManager = FindAnyObjectByType<UIManager>();
    }
    public void Interaction()
    {
        //uiManager.OpenObjectUIWithoutInv(this);
    }
}

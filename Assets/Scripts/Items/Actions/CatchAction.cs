using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchAction : ItemAction
{
    public GameObject catchAreaPrefab;
    private PlayerMovement playerTransfrom;
    public override void Action(InventoryItem equippedItem)
    {
        if(playerTransfrom == null){
            playerTransfrom = FindAnyObjectByType<PlayerMovement>();
        }
        if (catchAreaPrefab != null && playerTransfrom != null)
        {
            GameObject catchArea = Instantiate(catchAreaPrefab);
            catchArea.transform.position = playerTransfrom.transform.position;
            Debug.Log("Catch area instantiated");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CatchArea : MonoBehaviour
{
    public float lifeTime = 0.2f;
    private InventoryManager inventoryManager;
    private Transform playerTransfrom;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }
    private void Update()
    {
        if(playerTransfrom == null)
            playerTransfrom = FindAnyObjectByType<PlayerMovement>().transform;
        transform.position = playerTransfrom.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision != null && collision.GetComponent<FloatingItem>() != null)
        {
            inventoryManager.AddItem(collision.GetComponent<FloatingItem>().item,1);
            Destroy(collision.gameObject);
        }
    }
}

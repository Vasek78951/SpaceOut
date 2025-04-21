using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Item item;
    public SpriteRenderer spriteRenderer;
    public int count;
    public bool canPickUp = false;
    public float pickupDelay = 0.0f;

    private void Start()
    {
        StartCoroutine(EnablePickupAfterDelay());
    }
    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        spriteRenderer.sprite = newItem.icon;
        canPickUp = false;
        count = newItem.count;
    }
    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(pickupDelay);
        canPickUp = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Adjust based on collider size
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.blue, 1f);
        Debug.LogWarning("Collision with: " + collision.gameObject.name);
        if (collision.CompareTag("Player") && canPickUp)
        {
            InventoryManager inventory = FindAnyObjectByType<InventoryManager>();
            if (inventory != null)
            {
                Debug.Log(item);
                inventory.AddItem(item, count);
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<ConveyorBelt>() != null)
        {
            Debug.Log("moving");
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            rb.MovePosition(rb.position + new Vector2(0.1f, 0));
        }
    }
}

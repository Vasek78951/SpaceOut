using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public Item item;
    public SpriteRenderer spriteRenderer;
    [Range(0.0f, 3.0f)]
    public float minSpeed;
    [Range(0.0f, 3.0f)]
    public float maxSpeed;
    Rigidbody2D rb;
    public int TimeDespawn;
    public float rndSpeed;
    InventoryManager inventory;

    // Rotation variables
    public float minRotationSpeed = 10f;
    public float maxRotationSpeed = 50f;
    private float rotationSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rndSpeed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed); // Random rotation speed
        inventory = FindAnyObjectByType<InventoryManager>();
    }

    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        spriteRenderer.sprite = newItem.icon;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the item
        rb.MovePosition(rb.position + new Vector2(-rndSpeed * Time.fixedDeltaTime, 0));

        // Rotate the item continuously at random speed
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void Interaction()
    {
        inventory.AddItem(item, item.count);
        Destroy(gameObject);
    }
}


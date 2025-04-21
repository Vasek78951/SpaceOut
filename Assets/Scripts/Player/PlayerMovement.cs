using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CustomInput input;
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D rb;
    public float groundMoveSpeed = 5f; // Speed of player movement
    public float waterMoveSpeed = 3f;
    public GameObject inventory;
    Indicator indicator;
    public float raycastDistance = 1f;
    public bool isOnFloor;
    public LayerMask layerMask;
    public ContactFilter2D contactFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public bool isOnPlatform = false;
    public Vector2 overlapBoxSize = new Vector2(1.0f, 1.0f);
    public int zAxis = 0;
    public Grid grid;
    public bool isFalling;
    public float fallingDistance;
    private List<Collision2D> collisions = new List<Collision2D>();
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    // Timer variables
    private float timeOutsidePlatform = 0f;
    public float teleportDelay = 5f; // Set the delay (in seconds) for teleporting after being outside the platform

    private void Awake()
    {
        input = new CustomInput();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMovementPerformed;
        input.Player.Move.canceled += OnMovementCancelled;
        input.Player.IncreaseZ.performed += IcreaseZAxis;
        input.Player.DecreaseZ.performed += DecreaseZAxis;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= OnMovementPerformed;
        input.Player.Move.canceled -= OnMovementCancelled;
        input.Player.IncreaseZ.canceled -= IcreaseZAxis;
        input.Player.DecreaseZ.canceled -= DecreaseZAxis;
    }

    private void DecreaseZAxis(InputAction.CallbackContext context)
    {
        DecreaseZAxis();
    }

    private void IcreaseZAxis(InputAction.CallbackContext context)
    {
        IcreaseZAxis();
    }

    private void DecreaseZAxis()
    {
        zAxis--;
        grid.transform.position -= new Vector3(0, 1.5f, 0);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = zAxis.ToString();
        isOnPlatform = false;
        collisions.Clear();
    }

    private void IcreaseZAxis()
    {
        zAxis++;
        grid.transform.position += new Vector3(0, 1.5f, 0);
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = zAxis.ToString();
        isOnPlatform = false;
        collisions.Clear();
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (!isOnPlatform)
        {
            // Track time outside platform
            timeOutsidePlatform += Time.fixedDeltaTime;
            if (timeOutsidePlatform >= teleportDelay)
            {
                TeleportToCenter(); // Teleport the player to the center if they have been outside too long
                timeOutsidePlatform = 0f; // Reset the timer
            }
        }
        else
        {
            timeOutsidePlatform = 0f; // Reset the timer when back on platform
        }

        if (moveVector != Vector2.zero || isFalling)
        {
            bool success = TryMove(moveVector);
            if (!success)
            {
                success = TryMove(new Vector2(moveVector.x, 0));
            }
            if (!success)
            {
                success = TryMove(new Vector2(0, moveVector.y));
            }
            animator.SetBool("isMoving", success);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (moveVector.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveVector.x > 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void TeleportToCenter()
    {
        // Teleport to the center of the platform (you might adjust this to the actual center of the grid)
        Vector2 platformCenter = new Vector2(0f, 0f); // You can adjust this as needed
        transform.position = platformCenter;
        Debug.Log("Player teleported to the center of the platform.");
    }

    private bool TryMove(Vector2 moveVector)
    {
        float moveSpeed = isOnPlatform ? groundMoveSpeed : waterMoveSpeed;
        Vector2 moveAmount = moveVector.normalized * moveSpeed * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, moveVector.normalized, moveAmount.magnitude, layerMask);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<BuildingObject>().zAxis == zAxis)
        {
            return false; // Block movement if collision is detected
        }

        rb.MovePosition(rb.position + moveAmount);
        return true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform") && collision.gameObject.GetComponent<BuildingObject>().zAxis == zAxis)
        {
            isOnPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Platform") && other.gameObject.GetComponent<BuildingObject>().zAxis == zAxis)
        {
            isOnPlatform = false;
        }
    }
}

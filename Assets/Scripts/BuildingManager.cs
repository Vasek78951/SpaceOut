using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.GridBrushBase;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] LayerMask layerMaskPlatform;
    [SerializeField] LayerMask layerMaskWalls;
    [SerializeField] LayerMask layerMaskTopWalls;
    public PlayerMovement player;
    public buildingObjectDatabase database;
    public Indicator indicator;
    public int selectedObjectIndex = -1;
    public Grid grid;
    public InventoryManager inventoryManager;
    public Raft raft;
    private GameObject preview;
    public int rotationDirection;
    public bool canRotate;
    public int GetSelectedIndex()
    {
        return selectedObjectIndex;
    }

    public void Start()
    {
        StopPlacement();
        BuildInitialPlatform(5, 5);

    }

    public void StartPlacement(int ID)
    {
        StopPlacement();

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.Log($"No ID found {ID}");
            return;
        }

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;

        preview = Instantiate(database.objectsData[selectedObjectIndex].Object.Prefab, Vector2.zero, Quaternion.identity, raft.transform);
        preview.layer = 1;
        SpriteRenderer sprite = preview.GetComponent<SpriteRenderer>();
        if (sprite == null )
        {
            sprite = preview.GetComponentInChildren<SpriteRenderer>();
        }
        sprite.color = new Color(1, 1, 1, 0.1f);
        sprite.sortingOrder = 1;
        canRotate = true;   
    }

    private void Update()
    {

        if (selectedObjectIndex == 0)
        {
            FloorPreview();
        }

        if (selectedObjectIndex == 1)
        {
            WallPreview();
        }

        if (selectedObjectIndex == 2)
        {
            FloorPreview();
        }

        if (Input.GetKeyDown(KeyCode.R) && canRotate)
        {
            RotatePreview();
        }
    }
    private void RotatePreview()
    {
        if (selectedObjectIndex == 0)
            return;
        rotationDirection++;
        if (rotationDirection > 4)
        {
            rotationDirection = 1; // Reset to 1 after reaching 4
        }

        // Apply rotation to preview based on the direction
        float rotationAngle = 0f;
        switch (rotationDirection)
        {
            case 1:
                rotationAngle = 0f; // Up
                break;
            case 2:
                rotationAngle = 90f; // Right
                break;
            case 3:
                rotationAngle = 180f; // Down
                break;
            case 4:
                rotationAngle = 270f; // Left
                break;
        }
        preview.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }
    private void FloorPreview()
    {
        Vector3Int buildingPos3 = grid.WorldToCell(inputManager.GetSelectedPosition());
        Vector2 buildingPos = (Vector2)grid.CellToWorld(buildingPos3) + new Vector2(0.5f, 0.5f); // Convert cell position back to world position
        preview.transform.position = buildingPos;
        preview.GetComponentInChildren<SpriteRenderer>().sortingLayerName = player.zAxis.ToString();
        preview.GetComponent<BoxCollider2D>().enabled = false;
    }

    private GameObject WallPreview()
    {
        Vector2 mousePos = inputManager.GetSelectedPosition();
        Vector3Int cell = grid.WorldToCell(inputManager.GetSelectedPosition());
        Vector2 cellPos = (Vector2)grid.CellToWorld(cell) + new Vector2(0.5f, 0.5f);

        Vector2 buildingPos = Vector2.zero;
        if (rotationDirection == 1 || rotationDirection == 3)// ---------------------------------------
        {
            Destroy(preview);
            preview = Instantiate(database.objectsData[selectedObjectIndex].Object.Prefab, Vector2.zero, Quaternion.identity, raft.transform);
            preview.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if (rotationDirection == 1) 
            {
                buildingPos = new Vector2(cellPos.x, cellPos.y - grid.cellSize.y / 2 + database.objectsData[selectedObjectIndex].Object.Prefab.transform.localScale.y / 2);
            }
            else
            {
                buildingPos = new Vector2(cellPos.x, cellPos.y + grid.cellSize.y / 2 + database.objectsData[selectedObjectIndex].Object.Prefab.transform.localScale.y / 2);
            }
        }
        else // |||||||||||||||||||| 
        {
            Destroy(preview);
            preview = Instantiate(database.objectsData[selectedObjectIndex].Object.prefabSide, Vector2.zero, Quaternion.identity, raft.transform);
            preview.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if (rotationDirection == 2)
            {
                buildingPos = new Vector2(cellPos.x - grid.cellSize.x / 2, cellPos.y + (preview.transform.localScale.y - 1)/2 );
            }
            else
            {
                buildingPos = new Vector2(cellPos.x + grid.cellSize.x / 2, cellPos.y + (preview.transform.localScale.y - 1) / 2);
            }

        }
        preview.transform.position = buildingPos;

        Vector2 hitboxPos;
        Vector2 hitboxSize;

        if (rotationDirection == 1 || rotationDirection == 3)
        {
            hitboxPos = buildingPos - new Vector2(0, preview.transform.localScale.y / 2 - preview.GetComponent<BoxCollider2D>().size.y / 2);
            hitboxSize = preview.GetComponent<BoxCollider2D>().size / 2;
        }
        else
        {
            hitboxPos = buildingPos;
            hitboxSize = preview.GetComponent<BoxCollider2D>().size - new Vector2(1 - preview.transform.localScale.x / 2, preview.GetComponent<BoxCollider2D>().size.y / 2f);
        }


        // Check for colliders at the target position
        Collider2D[] colliders = Physics2D.OverlapBoxAll(hitboxPos, hitboxSize, 0, layerMaskWalls);
        foreach (Collider2D collider in colliders)
        {
            if (collider != null)
            {
                preview.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
                Debug.Log("Space occupied, cannot place.");

                return null;
            }
        }

        if (rotationDirection == 1 || rotationDirection == 3)
        {
            hitboxPos = buildingPos - new Vector2(0, preview.transform.localScale.y/2);
            hitboxSize = preview.GetComponent<BoxCollider2D>().size * new Vector2(0.8f, 1.5f);
        }
        else
        {
            hitboxPos = buildingPos;
            hitboxSize = preview.GetComponent<BoxCollider2D>().size - new Vector2(1 - preview.transform.localScale.x / 2, preview.GetComponent<BoxCollider2D>().size.y / 1.5f);
        }

        if (0 >= Physics2D.OverlapBoxAll(hitboxPos, hitboxSize, 0, layerMaskPlatform).Length)
        {
            preview.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
            Debug.Log("No adjacent tile found");
            return null;
        }

        preview.GetComponentInChildren<SpriteRenderer>().sortingLayerName = player.zAxis.ToString();
        return preview;
    }

    void OnDrawGizmos() 
    {
        // Check if preview exists and has a BoxCollider2D component
        if (preview != null && preview.GetComponent<BoxCollider2D>() != null)
        {
            // Get the hitbox size and position
            Vector2 hitboxPosition = preview.transform.position;
            Vector2 hitboxSize = new Vector2(1.5f, 1.5f);

            // Set the Gizmo color
            Gizmos.color = Color.green; // Change to any color you prefer

            // Draw the outline of the hitbox
            Gizmos.DrawWireCube(hitboxPosition, hitboxSize);
        }
    }


    private void PlaceStructure()
    {
        // Check if the player has the required resources
        if (!HasRequiredResources(database.objectsData[selectedObjectIndex].requiredItems))
        {
            Debug.Log("Not enough resources to build");
            return;
        }

        if (selectedObjectIndex == 0)
        {
            PlaceFloor();
        }
        if(selectedObjectIndex == 1)
        {
            PlaceWall();
        }


        // Deduct the resources
        DeductResources(database.objectsData[selectedObjectIndex].requiredItems);
    }

    private void PlaceFloor()
    {
        Vector3Int buildingPos3 = grid.WorldToCell(inputManager.GetSelectedPosition());
        Vector2 buildingPos = preview.transform.position; // Convert cell position back to world position
        Debug.Log("Building Position: " + buildingPos);

        // Check if the position is valid
        Vector2[] positions = new Vector2[]
        {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1),
        };

        bool hasAdjacentPlatform = false;
        foreach (Vector2 pos in positions)
        {
            Vector2 adjacentPos = buildingPos + pos;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(adjacentPos, new Vector2(0.5f, 0.5f), 0, layerMaskPlatform);
            if (0 < colliders.Length)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.GetComponent<BuildingObject>().zAxis == player.zAxis)
                    {
                        Debug.Log("Adjacent tile found");
                        hasAdjacentPlatform = true;
                        break;
                    }
                }
            }
        }


        //Check if there is wall under
        Collider2D[] colliders2 = Physics2D.OverlapBoxAll(buildingPos, new Vector2(1.5f, 1.5f), 0, layerMaskTopWalls);
        Debug.Log("hited colliders: " + colliders2.Length);
        foreach (Collider2D collider in colliders2)
        {
            if (collider.GetComponentInParent<BuildingObject>().zAxis == player.zAxis - 1)
            {
                Debug.Log("wall udner found");
                hasAdjacentPlatform = true;
                break;
            }
        }


        // Check for colliders at the target position
        Collider2D[] colliders3 = Physics2D.OverlapBoxAll(buildingPos, new Vector2(0.5f, 0.5f), 0, layerMaskPlatform);
        foreach (Collider2D collider in colliders3)
        {
            if (collider != null)
            {
                if (collider.GetComponent<BuildingObject>().zAxis == player.zAxis)
                {
                    Debug.Log("Space occupied, cannot place.");
                    return;
                }
            }
        }

        if (!hasAdjacentPlatform)
        {
            Debug.Log("No adjacent tile found");
            return;
        }

        // Instantiate at the calculated position if valid
        GameObject gameStructure = Instantiate(database.objectsData[selectedObjectIndex].Object.Prefab, buildingPos, Quaternion.identity, raft.transform);
        gameStructure.GetComponent<BuildingObject>().zAxis = player.zAxis;
        gameStructure.GetComponentInChildren<SpriteRenderer>().sortingLayerName = player.zAxis.ToString();
        raft.placedFloors.Add(buildingPos);
    }

    private void PlaceWall()
    {
        GameObject wall = Instantiate(WallPreview());
        wall.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        wall.GetComponent<SpriteRenderer>().sortingOrder = -1;
        wall.GetComponent<BuildingObject>().zAxis = player.zAxis;
        wall.GetComponentInChildren<SpriteRenderer>().sortingLayerName = player.zAxis.ToString();
        Debug.Log("builded at: " + wall.transform.position);

    }

    private bool HasRequiredResources(List<RequiredItem> requiredItems)
    {
        foreach (var item in requiredItems)
        {
            if (!inventoryManager.HasItem(item.item, item.amount))
            {
                return false;
            }
        }
        return true;
    }

    private void DeductResources(List<RequiredItem> requiredItems)
    {
        foreach (var item in requiredItems)
        {
            for(int i = 0; i < item.amount; i++)
            {
                inventoryManager.RemoveItem(item.item);
            }
            
        }
    }

    private void StopPlacement()
    {
        canRotate = false;
        selectedObjectIndex = -1;
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        Destroy(preview);
    }
    private void BuildInitialPlatform(int width, int height)
    {
        int startX = -width / 2;
        int startY = -height / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(startX + x, startY + y, 0);
                Vector2 worldPos = (Vector2)grid.CellToWorld(cellPos) + new Vector2(0.5f, 0.5f);

                GameObject floor = Instantiate(database.objectsData[0].Object.Prefab, worldPos, Quaternion.identity, raft.transform);
                raft.placedFloors.Add(worldPos);
                floor.GetComponentInChildren<SpriteRenderer>().sortingLayerName = player.zAxis.ToString();

                BuildingObject bo = floor.GetComponent<BuildingObject>();
                if (bo != null)
                {
                    bo.zAxis = player.zAxis;
                }
            }
        }
    }

}

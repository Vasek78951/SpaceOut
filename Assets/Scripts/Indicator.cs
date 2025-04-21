using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicatorPrefab; // Prefab for the cell indicator
    [SerializeField] private InputManager inputManager;
    [SerializeField] public Grid grid;
    public BuildingManager buildingManager;
    public buildingObjectDatabase database;
    private GameObject player;
    private GameObject cellIndicator;
    public bool canBuild;
    [SerializeField] private float maxRange = 10f; // Default max range
    public LayerMask layerMask;

    private void Start()
    {
        Inicialize();
    }

    public void Inicialize()
    {
        SpawnCellIndicator();
        player = FindPlayer();
    }

    private void Update()
    {
        UpdateIndicator();
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    private void SpawnCellIndicator()
    {
        if (cellIndicator != null)
        {
            Debug.LogWarning("Cell Indicator already exists.");
            return;
        }

        cellIndicator = Instantiate(cellIndicatorPrefab); // Instantiate from prefab
        cellIndicator.SetActive(false); // Set inactive initially
    }

    public GameObject GetObject()
    {
        Vector3 mousePosition = inputManager.GetSelectedPosition();
        Vector3 playerPosition = player.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, maxRange, layerMask);
        if (hit.collider == null)
        {
            return null;
        }
        if (Vector3.Distance(hit.collider.transform.position, playerPosition) > maxRange)
        {
            cellIndicator.SetActive(false);
            return null;
        }
        GameObject hitObject = hit.collider.gameObject;
        if (hitObject == null)
        {
            Debug.LogError("Hit object is null.");
            return null;
        }
        return hitObject;
    }

    private void UpdateIndicator()
    {
        if (cellIndicator == null)
        {
            Debug.LogError("Cell Indicator is not instantiated.");
            return;
        }

        canBuild = true;
        Vector3 mousePosition = inputManager.GetSelectedPosition();
        Vector3 playerPosition = player.transform.position;



        GameObject hitObject = GetObject();
        if (hitObject == null)
        {
            cellIndicator.SetActive(false);
            return;
            
        }

        cellIndicator.transform.position = hitObject.transform.position;
        var boxCollider = hitObject.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            cellIndicator.transform.localScale = new Vector2(boxCollider.size.x, boxCollider.size.y);
        }
        else
        {
            Debug.LogError("BoxCollider2D not found on hitObject.");
            return;
        }

        InteractiveObject interactiveObject = hitObject.GetComponentInParent<InteractiveObject>();

        if (interactiveObject != null)
        {
            cellIndicator.SetActive(true);
            // Interaction when 'E' is pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactiveObject.Interaction();
                Debug.Log("Started interaction");
            }
        }
        else
        {
            cellIndicator.SetActive(false);
        }

        // If a building object is selected, update the indicator's scale and position
        if (buildingManager.selectedObjectIndex > -1)
        {
            cellIndicator.transform.localScale = new Vector2(
            //database.objectsData[buildingManager.selectedObjectIndex].Size.x,
            //database.objectsData[buildingManager.selectedObjectIndex].Size.y
            );
            cellIndicator.transform.position = GetCellIndicatorPos();
            //cellIndicator.SetActive(true);
        }

    }

    public Vector2 GetCellIndicatorPos()
    {
        Vector2 mousePosition = inputManager.GetSelectedPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 cellIndicatorPos = grid.CellToWorld(gridPosition);
        cellIndicatorPos += new Vector3(0.5f, 0.5f, 0); // Centering the indicator
        return cellIndicatorPos;
    }
}

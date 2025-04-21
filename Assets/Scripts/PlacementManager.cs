using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public Raft raft;
    public PlacementObjectDatabase database;
    public InputManager inputManager;
    public Indicator indicator;
    public LayerMask layerMask;
    public int selectedObjectIndex = -1;
    public CableManager cableManager;
    private GameObject previewObject;

    private void Start()
    {
        StopBuilding();
        //raft.placedBuildings.Add(new Vector2(0, 0));
    }

    private void OnDestroy()
    {
        StopBuilding();
    }

    private void OnPlacement(Vector2 buildingPos, int selectedObjectIndex)
    {
        if (raft != null && selectedObjectIndex >= 0)
        {
            // Instantiate the prefab and set the raft as its parent
            GameObject prefab = database.palcementObjectsData[selectedObjectIndex].Prefab;
            if (prefab == null)
            {
                Debug.LogError("Prefab not found for index: " + selectedObjectIndex);
                return;
            }

            // Convert world position to local position relative to the raft
            Vector3 localPosition = raft.transform.InverseTransformPoint(new Vector3(buildingPos.x, buildingPos.y, 0));

            GameObject gameStructure = Instantiate(prefab, raft.transform); 
            gameStructure.transform.localPosition = localPosition;
            raft.placedBuildings.Add(new Vector2(buildingPos.x, buildingPos.y));
            if (gameStructure.GetComponent<CablePiece>() != null)
                cableManager.InstantiateCabel(new Vector2Int((int)localPosition.x, (int)localPosition.y), gameStructure.GetComponent<CablePiece>());
        }
    }

    public void StartBuilding(int ID)
    {
        StopBuilding();
        selectedObjectIndex = database.palcementObjectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.Log($"No ID found for {ID}");
            return;
        }
        inputManager.OnClicked += RequestBuild;
        inputManager.OnExit += StopBuilding;
        GameObject prefab = database.palcementObjectsData[selectedObjectIndex].Prefab;
        previewObject = Instantiate(prefab);
        previewObject.layer = 0;
        previewObject.tag = "Untagged";
        Destroy(previewObject.GetComponent<BoxCollider2D>());
        SetTransparent(previewObject);

    }

    public void RequestBuild()
    {
        if (inputManager.isPointerOverUI())
        {
            return;
        }

        Vector2 buildingPos = indicator.GetCellIndicatorPos();
        Build(buildingPos, selectedObjectIndex);
    }

    private void Build(Vector2 buildingPos, int selectedObjectIndex)
    {
        if (raft.placedBuildings.Contains(buildingPos))
        {
            Debug.Log("This position is already occupied!");
            return;
        }
        if (!raft.placedFloors.Contains(buildingPos))
        {
            Debug.Log("This position is already occupied!");
            return;
        }

        OnPlacement(buildingPos, selectedObjectIndex);
    }
    private void Update()
    {
        if (previewObject != null)
        {
            Vector2 previewPos = indicator.GetCellIndicatorPos();
            previewObject.transform.position = new Vector3(previewPos.x, previewPos.y, 0f);
        }
    }

    private void StopBuilding()
    {
        selectedObjectIndex = -1;
        inputManager.OnClicked -= RequestBuild;
        inputManager.OnExit -= StopBuilding;
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }
    private void SetTransparent(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.5f; // 50% transparent
            renderer.color = color;
        }
    }

}

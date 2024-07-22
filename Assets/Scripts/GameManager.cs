using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Grid terrainGrid;
    [SerializeField] private Tilemap roadTilemap;

    [SerializeField] private RuleTile dirtRoadTile;
    [SerializeField] private GameObject ghostIndicator;

    private bool isPlacingRoad = false;
    private float cellOffset = 0.5f;
    private float cellLayerOffset = -7;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            ToggleGhostActive();
        }

        if (isPlacingRoad)
        {
            Vector3Int currentCell = GetCurrentCell();
            UpdateGhostPosition(currentCell);

            if (Input.GetMouseButtonDown(0))
            {
                PlaceRoad(currentCell, dirtRoadTile);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                RemoveRoad(currentCell);
            }
        }
        
    }

    void ToggleGhostActive()
    {
        if (isPlacingRoad)
        {
            ghostIndicator.SetActive(false);
            isPlacingRoad = false;
        } 
        else
        {
            ghostIndicator.SetActive(true);
            isPlacingRoad = true;
        }
    }

    void UpdateGhostPosition(Vector3Int currentCell)
    {
        Vector3 currentCoordinates = GetCurrentCellPosition(currentCell);
        ghostIndicator.transform.position = new Vector3(currentCoordinates.x + cellOffset, currentCoordinates.y + cellOffset, cellLayerOffset);
    }

    void PlaceRoad(Vector3Int currentCell, RuleTile roadTile)
    {
        roadTilemap.SetTile(currentCell, roadTile);
    }

    void RemoveRoad(Vector3Int currentCell)
    {
        roadTilemap.SetTile(currentCell, null);
    }

    // Acquire the cell the mouse is currently hovered over
    private Vector3Int GetCurrentCell()
    {
        Vector2 gridPos = new Vector2(mainCamera.ScreenToWorldPoint(Input.mousePosition).x, mainCamera.ScreenToWorldPoint(Input.mousePosition).y);
        Vector3Int cellLocation = terrainGrid.WorldToCell(gridPos);

        return cellLocation;
    }

    // Acquire the global coordinates of the cell the mouse is currently hovered over
    private Vector3 GetCurrentCellPosition(Vector3Int currentCell)
    {
        Vector3 cellPositionNormalized = terrainGrid.CellToWorld(currentCell);

        return cellPositionNormalized;
    }
}

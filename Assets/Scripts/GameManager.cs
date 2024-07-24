using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIGameManager gameManagerUI;
    [SerializeField] private TerrainGenerator worldManager;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private Grid terrainGrid;
    [SerializeField] private Tilemap roadTilemap;

    [SerializeField] private RuleTile dirtRoadTile;
    [SerializeField] private GameObject ghostIndicator;

    private bool isPlacingRoad = false;
    private float cellOffset = 0.5f;
    private float cellLayerOffset = -7;

    private List<Vector2> villageCoordinates = new List<Vector2>();
    private List<Vector2> roadCoordinates = new List<Vector2>();

    public bool isGameOver { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("There are " + villageCoordinates.Count + " villages."); 
        Debug.Log(villageCoordinates);
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
                if (!isGameOver && AllVillagesHavePath())
                {
                    Debug.Log("Congratulations!");
                }
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
        roadCoordinates.Add(new Vector2(currentCell.x, currentCell.y));
    }

    void RemoveRoad(Vector3Int currentCell)
    {
        roadTilemap.SetTile(currentCell, null);
        roadCoordinates.Remove(new Vector2(currentCell.x, currentCell.y));
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


    private bool AllVillagesHavePath()
    {
        foreach (Vector2 villageCoordinate in villageCoordinates)
        {
            // This is a placeholder function for later
            // Eventually, pathfinding will be enabled here to ensure that each village can be reached by roads
            if(!roadCoordinates.Contains(villageCoordinate))
            {
                // Note: it may not be necessary eventually to set isGameOver to false here
                // in the future, as there might be a post-game where this isn't a factor
                isGameOver = false;
                return false;
            }
        }

        isGameOver = true;
        return true;
    }



    public void ResetVillageList()
    {
        villageCoordinates.Clear();
    }

    public void AddVillage(int x, int y)
    {
        Vector2 coordinates = new Vector2(x, y);
        villageCoordinates.Add(coordinates);
    }

    public void AddVillage(Vector2 coordinates)
    {
        villageCoordinates.Add(coordinates);
    }

    public void AddVillage(Vector3Int coordinates)
    {
        Vector2 newCoordinates = new Vector2(coordinates.x, coordinates.y);
        villageCoordinates.Add(newCoordinates);
    }
    
}

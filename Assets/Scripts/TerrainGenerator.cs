using System.Collections;
using System.Collections.Generic;
using System.IO;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    private Pathfinding pathfinding;
    [SerializeField] private GameObject gameGrid;

    [SerializeField] private Grid terrainGrid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap forestTilemap;
    [SerializeField] private Tilemap lakeTilemap;
    [SerializeField] private Tilemap mountainTilemap;
    [SerializeField] private Tilemap villageTilemap;
    [SerializeField] private Tilemap roadTilemap;

    [SerializeField] private RuleTile baseGrassTile;
    [SerializeField] private RuleTile mildGrassTile;
    [SerializeField] private RuleTile heavyGrassTile;
    [SerializeField] private RuleTile heavyForestRuleTile;
    [SerializeField] private RuleTile lightForestRuleTile;
    [SerializeField] private RuleTile mountainRuleTile;
    [SerializeField] private RuleTile hillRuleTile;
    [SerializeField] private RuleTile lakeRuleTile;
    [SerializeField] private RuleTile villageRuleTile;

    [SerializeField] private float terrainOffsetX = 50f;
    [SerializeField] private float terrainOffsetY = 50f;

    [SerializeField] private float noiseFrequency = 9f;

    [SerializeField] private float heavyGrassThreshold = 0.75f;
    [SerializeField] private float mildGrassThreshold = 0.5f;

    [SerializeField] private float mountainThreshold = 0.8f;
    [SerializeField] private float heavyForestThreshold = 0.6f;
    [SerializeField] private float hillsThreshold = 0.415f;
    [SerializeField] private float grassTopTheshold = 0.4f;
    [SerializeField] private float lightForestTopThreshold = 0.35f;
    [SerializeField] private float lightForestBottomThreshold = 0.335f;
    [SerializeField] private float grassBottomThreshold = 0.22f;
    [SerializeField] private float villageThreshold = 0.2f;
    [SerializeField] private float waterThreshold = 0.175f;

    // These will likely eventually be shifted to a persistent data class at some point
    private int seedMin = 10000;
    private int seedMax = 10000000;
    public int mapSeed {  get; private set; }
    public bool isMapSet {  get; private set; }

    //ENCAPSULATION
    private int m_Width = 40;
    public int width
    {
        get { return m_Width; }
        set
        {
            if (value < 10 && !isMapSet)
            {
                Debug.Log("Cannot have a grid width smaller than 10");
                m_Width = 10;
            }
            else if (value > 255 && !isMapSet)
            {
                Debug.Log("Cannot have a grid width greater than 255");
                m_Width = 255;
            }
            else if (!isMapSet)
            {
                m_Width = value;
            }
        }
    }

    //ENCAPSULATION
    private int m_Height = 30;
    public int height
    {
        get { return m_Height; }
        set
        {
            if (value < 10)
            {
                Debug.Log("Cannot have a grid height smaller than 10");
                m_Height = 10;
            }
            else if (value > 255)
            {
                Debug.Log("Cannot have a grid height greater than 255");
                m_Height = 255;
            }
            else
            {
                m_Height = value;
            }
        }
    }

    public int gridPadding = 1;

    // Start is called before the first frame update
    void Awake()
    {
        mapSeed = GenerateSeed();
        LoadGrid(true);
    }

    private void Update()
    {

    }



    public void GenerateTerrain(bool isMakingRiver = false)
    {
        GenerateGroundLayer();
        GenerateTerrainLayer(isMakingRiver);
    }

    public int GenerateSeed()
    {
        if (DataManager.Instance != null && DataManager.Instance.IsGameLoaded)
        {
            return DataManager.Instance.Seed;
        } 
        else
        {
            return Random.Range(seedMin, seedMax);
        }
    }


    private void LoadGrid(bool isMakingRiver = false)
    {
        if (DataManager.Instance != null && DataManager.Instance.IsGameLoaded)
        {
            terrainGrid = gameGrid.GetComponent<Grid>();
            groundTilemap = gameGrid.transform.GetChild(0).GetComponent<Tilemap>();
            forestTilemap = gameGrid.transform.GetChild(1).GetComponent<Tilemap>();
            lakeTilemap = gameGrid.transform.GetChild(2).GetComponent<Tilemap>();
            mountainTilemap = gameGrid.transform.GetChild(3).GetComponent<Tilemap>();
            villageTilemap = gameGrid.transform.GetChild(4).GetComponent<Tilemap>();

            width = DataManager.Instance.Width;
            height = DataManager.Instance.Height;
            gridPadding = DataManager.Instance.Padding;

            pathfinding = new Pathfinding(this, terrainGrid, width + (2 * gridPadding), height + (2 * gridPadding));

            LoadFromSavedGrid();

            isMapSet = true;
        } 
        else
        {
            GenerateTerrain(isMakingRiver);
        }
    }


    private void GenerateGroundLayer()
    {
        for (int i = 0;  i < width + 2 * gridPadding; i++) 
        { 
            for (int j = 0; j < height + 2 * gridPadding; j++)
            {
                float noiseValue = Mathf.PerlinNoise((i + mapSeed) / noiseFrequency, (j + mapSeed) / noiseFrequency);
                //Debug.Log(noiseValue + $" at position {i}, {j}");
                if (noiseValue > heavyGrassThreshold)
                {
                    PlaceTile(groundTilemap, heavyGrassTile, i, j, (int)TileType.grassHeavy);
                } 
                else if (noiseValue > mildGrassThreshold)
                {
                    PlaceTile(groundTilemap, mildGrassTile, i, j, (int)TileType.grassMild);
                }
                else
                {
                    PlaceTile(groundTilemap, baseGrassTile, i, j, (int)TileType.grass);
                }
            }
        }
        Debug.Log("Ground generated!");
    }

    private void GenerateTerrainLayer(bool isMakingRiver = false)
    {
        float startMin = int.MaxValue;
        float endMin = int.MaxValue;
        int startX = 0;
        int startY = 0;
        int endX = 0; 
        int endY = 0;

        for (int i = 0; i < width + 2 * gridPadding; i++)
        {
            for (int j = 0; j < height + 2 * gridPadding; j++)
            {
                float noiseValue = Mathf.PerlinNoise(((i + mapSeed) / noiseFrequency) + terrainOffsetX, ((j + mapSeed) / noiseFrequency) + terrainOffsetY);
                //Debug.Log(noiseValue + $" at position {i}, {j}");

                // To make this look graphically smoother, forest is to be placed before mountains
                if (noiseValue > heavyForestThreshold)
                {
                    PlaceTile(forestTilemap, heavyForestRuleTile, i, j, (int)TileType.heavyForest);
                } 
                else if (noiseValue > lightForestBottomThreshold && noiseValue < lightForestTopThreshold)
                {
                    PlaceTile(forestTilemap, lightForestRuleTile, i, j, (int)TileType.lightForest);
                }

                if (noiseValue > mountainThreshold)
                {
                    PlaceTile(mountainTilemap, mountainRuleTile, i, j, (int)TileType.mountain);
                } 
                else if (noiseValue > grassTopTheshold && noiseValue < hillsThreshold)
                {
                    PlaceTile(mountainTilemap, hillRuleTile, i, j, (int)TileType.hill);
                }

                if (noiseValue < grassBottomThreshold && noiseValue > villageThreshold && (i < (width + gridPadding) && i >= gridPadding) && (j < (height + gridPadding) && j >= gridPadding))
                {
                    PlaceTile(villageTilemap, villageRuleTile, i, j, (int)TileType.village);
                }

                if (noiseValue < waterThreshold) 
                {
                    PlaceTile(lakeTilemap, lakeRuleTile, i, j, (int)TileType.lake);
                }



                if (isMakingRiver && i == 0)
                {
                    if(noiseValue < startMin)
                    {
                        startMin = noiseValue;
                        startX = i;
                        startY = j;
                    }
                }
                else if (isMakingRiver && i == (width + (2 * gridPadding) - 1))
                {
                    if (noiseValue < endMin)
                    {
                        endMin = noiseValue;
                        endX = i;
                        endY = j;
                    }
                }
            }
        }

        // Note: the pathfinding algorithm needs to be initialized *AFTER* the terrain grid so the
        // terrain costs can be properly calculated.  This is to lower computing costs.
        if (pathfinding == null)
        {
            pathfinding = new Pathfinding(this, terrainGrid, width + (2 * gridPadding), height + (2 * gridPadding));
        }

        if(isMakingRiver)
        {
            PlaceRiver(pathfinding.FindPath(startX, startY, endX, endY, false, 0));
        }

        Debug.Log("Terrain generated!");
        isMapSet = true;

        if(DataManager.Instance != null)
        {
            SetGameTerrainData();
        }
        
    }


    //POLYMORPHISM
    // Place tiles in accordance to a given TileBase, layer, and grid position.  Set the key of the given object.
    private void PlaceTile(Tilemap tilemap, TileBase tile, int gridX, int gridY, int key)
    {
        Vector3Int coordinates = new Vector3Int(gridX, gridY);
        tilemap.SetTile(coordinates, tile);

        tilemap.GetInstantiatedObject(coordinates).GetComponent<TerrainFeature>().SetTerrainKey(key);
    }

    private void PlaceTile(Tilemap tilemap, SaveableTileData saveableTile)
    {
        bool isTilePlaced = true;
        switch (saveableTile.type)
        {
            case (int)TileType.grass:
                {
                    tilemap.SetTile(saveableTile.coordinates, baseGrassTile);
                    break;
                }
            case (int)TileType.grassMild:
                {
                    tilemap.SetTile(saveableTile.coordinates, mildGrassTile);
                    break;
                }
            case (int)TileType.grassHeavy:
                {
                    tilemap.SetTile(saveableTile.coordinates, heavyGrassTile);
                    break;
                }
            case (int)TileType.lightForest:
                {
                    tilemap.SetTile(saveableTile.coordinates, lightForestRuleTile);
                    break;
                }
            case (int)TileType.heavyForest:
                {
                    tilemap.SetTile(saveableTile.coordinates, heavyForestRuleTile);
                    break;
                }
            case (int)TileType.mountain:
                {
                    tilemap.SetTile(saveableTile.coordinates, mountainRuleTile);
                    break;
                }
            case (int)TileType.hill:
                {
                    tilemap.SetTile(saveableTile.coordinates, hillRuleTile);
                    break;
                }
            case (int)TileType.lake:
                {
                    tilemap.SetTile(saveableTile.coordinates, lakeRuleTile);
                    break;
                }
            case (int)TileType.village:
                {
                    tilemap.SetTile(saveableTile.coordinates, villageRuleTile);
                    break;
                }
            case (int)TileType.road:
                {
                    Debug.Log("[PLACEHOLDER] Road placed!");
                    break;
                }
            case (int)TileType.bridge:
                {
                    Debug.Log("[PLACEHOLDER] Bridge placed!");
                    break;
                }
            default:
                {
                    isTilePlaced = false;
                    Debug.Log("Error loading grid tile: Invalid tile key");
                    break;
                }
        }

        if(isTilePlaced)
        {
            SetTileDataFromLoadedData(tilemap, saveableTile);
        }
    }


    private void PlaceRiver(List<TerrainFeatureData> path)
    {
        foreach (TerrainFeatureData pathNode in path)
        {
            Vector3Int gridCoordinates = new Vector3Int(pathNode.xPos, pathNode.yPos);

            for(int i = 1; i < terrainGrid.transform.childCount; i++)
            {
                terrainGrid.transform.GetChild(i).GetComponent<Tilemap>().SetTile(gridCoordinates, null);
            }

            PlaceTile(lakeTilemap, lakeRuleTile, pathNode.xPos, pathNode.yPos, (int)TileType.lake);
        }

        RefreshGridTiles();
    }


    public Vector3Int GetGridCoordinates(Vector2 gridPosition)
    {
        return groundTilemap.WorldToCell(gridPosition);
    }
    
    public Vector2 GetGridPosition(Vector3Int gridCoordinates)
    {
        return groundTilemap.CellToWorld(gridCoordinates);
    }

    public TileBase GetTileAtPosition(Tilemap tilemap, Vector3 position)
    {
        return tilemap.GetTile(GetGridCoordinates(position));
    }



    public void HandleTileSelection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<GameObject> objectsAtLocation = new List<GameObject>();

        for (int i = 1; i < terrainGrid.transform.childCount; i++)
        {
            Tilemap tilemap = terrainGrid.transform.GetChild(i).gameObject.GetComponent<Tilemap>();
            GameObject instantiatedObject = tilemap.GetInstantiatedObject(GetGridCoordinates(mousePos));

            if (instantiatedObject != null)
            {
                objectsAtLocation.Add(instantiatedObject);
                Debug.Log("Object added: " + instantiatedObject.name);
            }
        }

        Debug.Log("Total objects at location: " + objectsAtLocation.Count);
    }

    public TerrainFeature SelectPathfindingTile(Tilemap tilemapLayer, int xPos, int yPos)
    {
        Vector3Int coordinates = new Vector3Int(xPos, yPos);
        GameObject instantiatedObject = tilemapLayer.GetInstantiatedObject(coordinates);

        if (instantiatedObject != null)
        {
            return instantiatedObject.GetComponent<TerrainFeature>();
        }

        return null;
    }

    public void RefreshGridTiles()
    {
        for (int i = 1; i < terrainGrid.transform.childCount; i++)
        {
            terrainGrid.transform.GetChild(i).GetComponent<Tilemap>().RefreshAllTiles();
        }
    }


    // Data manager helper function
    public void SetGameTerrainData()
    {
        DataManager.Instance.Seed = mapSeed;
        DataManager.Instance.Width = width;
        DataManager.Instance.Height = height;
        DataManager.Instance.Padding = gridPadding;

        SetGridTileData();
    }

    // Set the tile data for the DataManager
    // NOTE: This should ideally only be called when the game is saved and/or the terrain is generated so as to minimize lag time, and ideally in its own asynchronous call
    public void SetGridTileData()
    {
        DataManager.Instance.TileData.Clear();
        for (int i = 0; i < terrainGrid.transform.childCount - 1; i++)
        {
            Tilemap currentLayer = terrainGrid.transform.GetChild(i).GetComponent<Tilemap>();
            for (int xPos = 0; xPos < width + gridPadding; xPos++)
            {
                for (int yPos = 0; yPos < height + gridPadding; yPos++)
                {
                    Vector3Int coordinates = new Vector3Int(xPos, yPos);

                    if (currentLayer.HasTile(coordinates))
                    {
                        GameObject currentTile = currentLayer.GetInstantiatedObject(coordinates);
                        SaveableTileData tileData = new SaveableTileData();

                        if(currentTile.GetComponent<MountainFeature>() != null)
                        {
                            tileData.ore = currentTile.GetComponent<MountainFeature>().ore;
                        }
                        else if(currentTile.GetComponent<ForestFeature>() != null)
                        {
                            tileData.wood = currentTile.GetComponent<ForestFeature>().wood;
                        }
                        else if(currentTile.GetComponent<VillageFeature>() != null)
                        {
                            tileData.population = currentTile.GetComponent<VillageFeature>().population;
                        }

                        TerrainFeature feature = currentTile.GetComponent<TerrainFeature>();
                        tileData.coordinates = coordinates;
                        tileData.layer = i;
                        tileData.type = feature.terrainKey;

                        DataManager.Instance.TileData.Add(tileData);
                    }
                }
            }
        }
    }

    // Generate the grid based off of the passed-in data from DataManager
    // NOTE: This should only be called at the beginning of the screen or otherwise asynchronously in its own thread
    public void LoadFromSavedGrid()
    {
        for(int i = 0; i < terrainGrid.transform.childCount; i++)
        {
            Tilemap currentLayer = terrainGrid.transform.GetChild(i).gameObject.GetComponent<Tilemap>();
            List<SaveableTileData> layerTiles = DataManager.Instance.TileData.FindAll(x => x.layer == i);
            foreach (SaveableTileData tile in layerTiles)
            {
                PlaceTile(currentLayer, tile);
            }
        }
    }

    private void SetTileDataFromLoadedData(Tilemap tilemap, SaveableTileData saveableTile)
    {
        GameObject currentTile = tilemap.GetInstantiatedObject(saveableTile.coordinates);

        if (currentTile.GetComponent<MountainFeature>() != null)
        {
            currentTile.GetComponent<MountainFeature>().ore = saveableTile.ore;
        }
        else if (currentTile.GetComponent<ForestFeature>() != null)
        {
            currentTile.GetComponent<ForestFeature>().wood = saveableTile.wood;
        }
        else if (currentTile.GetComponent<VillageFeature>() != null)
        {
            currentTile.GetComponent<VillageFeature>().population = saveableTile.population;
        }

        TerrainFeature feature = currentTile.GetComponent<TerrainFeature>();
        feature.SetTerrainKey(saveableTile.type);
    }
}

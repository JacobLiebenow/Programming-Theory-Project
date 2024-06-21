using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    private Pathfinding pathfinding;

    [SerializeField] private Grid terrainGrid;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap forestTilemap;
    [SerializeField] private Tilemap lakeTilemap;
    [SerializeField] private Tilemap mountainTilemap;
    [SerializeField] private Tilemap villageTilemap;

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
        GenerateTerrain(true);

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            HandleTileSelection();
        }
    }



    public void GenerateTerrain(bool isMakingRiver = false)
    {
        GenerateGroundLayer();
        GenerateTerrainLayer(isMakingRiver);
    }

    public int GenerateSeed()
    {
        return Random.Range(seedMin, seedMax);
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
                    PlaceTile(groundTilemap, heavyGrassTile, i, j);
                } 
                else if (noiseValue > mildGrassThreshold)
                {
                    PlaceTile(groundTilemap, mildGrassTile, i, j);
                }
                else
                {
                    PlaceTile(groundTilemap, baseGrassTile, i, j);
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
                    PlaceTile(forestTilemap, heavyForestRuleTile, i, j);
                } 
                else if (noiseValue > lightForestBottomThreshold && noiseValue < lightForestTopThreshold)
                {
                    PlaceTile(forestTilemap, lightForestRuleTile, i, j);
                }

                if (noiseValue > mountainThreshold)
                {
                    PlaceTile(mountainTilemap, mountainRuleTile, i, j);
                } 
                else if (noiseValue > grassTopTheshold && noiseValue < hillsThreshold)
                {
                    PlaceTile(mountainTilemap, hillRuleTile, i, j);
                }

                if (noiseValue < grassBottomThreshold && noiseValue > villageThreshold && (i < (width + (2 * gridPadding) - 1) && i >= gridPadding) && (j < (height + (2 * gridPadding) - 1) && j >= gridPadding))
                {
                    PlaceTile(villageTilemap, villageRuleTile, i, j);
                }

                if (noiseValue < waterThreshold) 
                {
                    PlaceTile(lakeTilemap, lakeRuleTile, i, j);
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
    }



    private void PlaceTile(Tilemap tilemap, TileBase tile, int gridX, int gridY)
    {
        Vector3Int coordinates = new Vector3Int(gridX, gridY);
        tilemap.SetTile(coordinates, tile);
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

            lakeTilemap.SetTile(gridCoordinates, lakeRuleTile);
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

    private void RefreshGridTiles()
    {
        for (int i = 1; i < terrainGrid.transform.childCount; i++)
        {
            terrainGrid.transform.GetChild(i).GetComponent<Tilemap>().RefreshAllTiles();
        }
    }

}

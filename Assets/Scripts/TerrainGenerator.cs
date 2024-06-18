using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Awake()
    {

        mapSeed = GenerateSeed();
        GenerateTerrain();

        pathfinding = new Pathfinding(this, terrainGrid, width, height);

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            HandleTileSelection();
        }
    }



    public void GenerateTerrain()
    {
        GenerateGroundLayer();
        GenerateTerrainLayer();
    }

    public int GenerateSeed()
    {
        return Random.Range(seedMin, seedMax);
    }

    private void GenerateGroundLayer()
    {
        for (int i = 0;  i < width; i++) 
        { 
            for (int j = 0; j < height; j++)
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

    private void GenerateTerrainLayer()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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

                if (noiseValue < grassBottomThreshold && noiseValue > villageThreshold)
                {
                    PlaceTile(villageTilemap, villageRuleTile, i, j);
                }

                if (noiseValue < waterThreshold) 
                {
                    PlaceTile(lakeTilemap, lakeRuleTile, i, j);
                }
            }
        }
        Debug.Log("Terrain generated!");
    }



    private void PlaceTile(Tilemap tilemap, TileBase tile, int gridX, int gridY)
    {
        Vector3Int coordinates = new Vector3Int(gridX, gridY);
        tilemap.SetTile(coordinates, tile);
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

        Debug.Log("Fastest distance from origin: " + pathfinding.FindPath(0, 0, GetGridCoordinates(mousePos).x, GetGridCoordinates(mousePos).y));
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
}

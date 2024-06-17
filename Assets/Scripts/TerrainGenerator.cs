using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap terrainTilemap;

    [SerializeField] private Tile baseGrassTile;
    [SerializeField] private Tile mildGrassTile;
    [SerializeField] private Tile heavyGrassTile;
    [SerializeField] private RuleTile heavyForestRuleTile;
    [SerializeField] private RuleTile mountainRuleTile;
    [SerializeField] private RuleTile lakeRuleTile;
    [SerializeField] private RuleTile villageRuleTile;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float terrainOffsetX = 50f;
    [SerializeField] private float terrainOffsetY = 50f;

    [SerializeField] private float noiseFrequency = 5f;

    [SerializeField] private float heavyGrassThreshold = 0.75f;
    [SerializeField] private float mildGrassThreshold = 0.5f;

    [SerializeField] private float mountainThreshold = 0.8f;
    [SerializeField] private float heavyForestThreshold = 0.6f;
    [SerializeField] private float grassThreshold = 0.3f;
    [SerializeField] private float villageThreshold = 0.2f;
    [SerializeField] private float riverThreshold = 0.1f;

    private int seedMin = 10000;
    private int seedMax = 10000000;
    public int mapSeed {  get; [SerializeField] private set; }

    // Start is called before the first frame update
    void Start()
    {

        mapSeed = GenerateSeed();

        GenerateTerrain();

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
                Debug.Log(noiseValue + $"at position {i}, {j}");
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
                Debug.Log(noiseValue + $"at position {i}, {j}");
                if (noiseValue > mountainThreshold)
                {
                    PlaceTile(terrainTilemap, mountainRuleTile, i, j);
                }
                else if (noiseValue > heavyForestThreshold)
                {
                    PlaceTile(terrainTilemap, heavyForestRuleTile, i, j);
                } 
                else if (noiseValue < grassThreshold && noiseValue > villageThreshold)
                {
                    PlaceTile(terrainTilemap, villageRuleTile, i, j);
                }
                else if (noiseValue < riverThreshold) 
                {
                    PlaceTile(terrainTilemap, lakeRuleTile, i, j);
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

    private void PlaceMountain(Vector2 mountainPosition)
    {

    }

    private void PlaceForest(Vector2 forestPosition)
    {

    }

    private void PlaceRiver(Vector2 riverPosition)
    {

    }

    public void PlaceVillage(Vector2 villagePosition)
    {

    }


    public Vector3Int GetGridCoordinates(Vector2 gridPosition)
    {
        return terrainTilemap.WorldToCell(gridPosition);
    }
    
    public Vector2 GetGridPosition(Vector3Int gridCoordinates)
    {
        return terrainTilemap.CellToWorld(gridCoordinates);
    }

    public TileBase GetTileAtPosition(Tilemap tilemap, Vector3 position)
    {
        return tilemap.GetTile(GetGridCoordinates(position));
    }

    public void HandleTileSelection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject specificTile = terrainTilemap.GetObjectToInstantiate(GetGridCoordinates(mousePos));

        if (specificTile != null)
        {
            Debug.Log(specificTile.name);
        }
        else
        {
            Debug.Log("No terrain feature present!");
        }
    }
}

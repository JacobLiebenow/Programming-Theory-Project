using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap terrainTilemap;

    [SerializeField] private RuleTile heavyForestRuleTile;
    [SerializeField] private RuleTile mountainRuleTile;
    [SerializeField] private RuleTile riverRuleTile;
    [SerializeField] private RuleTile villageRuleTile;

    [SerializeField] private int width;
    [SerializeField] private int height;

    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrain();
    }
    
    public void GenerateTerrain()
    {
        GenerateGroundLayer();
        GenerateTerrainLayer();
    }

    private void GenerateGroundLayer()
    {
        Debug.Log("Ground generated!");
    }

    private void GenerateTerrainLayer()
    {
        Debug.Log("Terrain generated!");
    }

    private void PlaceTile(Vector2 tilePosition)
    {

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
    
}

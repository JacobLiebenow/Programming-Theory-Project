using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainFeature : MonoBehaviour 
{
    public int riverCost = 0;
    public int roadCost = 0;
    public int terrainKey {  get; private set; }
        
    public bool hasWater = false;

    public void SetTerrainKey(int key)
    {
        terrainKey = key;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableTileData
{
    public Vector3Int coordinates;
    public int layer;
    public int type;
    
    public int wood = 0;
    public int ore = 0;
    public int population = 0;
}

[Serializable]
public enum TilemapLayer
{
    groundLayer = 0,
    forestLayer = 1,
    mountainLayer = 2,
    waterLayer = 3,
    villageLayer = 4,
    roadLayer = 5
}

[Serializable]
public enum TileType
{
    grass = 0,
    grassMild = 1,
    grassHeavy = 2,

    lightForest = 100,
    heavyForest = 101,

    hill = 200,
    mountain = 201,

    lake = 300,

    village = 400,

    road = 500,
    bridge = 501
}


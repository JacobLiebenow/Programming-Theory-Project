using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainFeatureData
{
    public int xPos;
    public int yPos;

    public int m_riverCost = 0;
    public int m_roadCost = 0;
    public int m_gCost = 0;
    public int m_hCost = 0;
    public int m_fCost = 0;

    public bool hasWater = false;

    public TerrainFeatureData cameFromFeature;

    public TerrainFeatureData(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    public void CalculateFCost()
    {
        m_fCost = m_gCost + m_hCost;
    }
}

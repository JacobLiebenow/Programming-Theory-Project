using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int DEFAULT_RIVER_COST = 100;
    private const int DEFAULT_ROAD_COST = 10;

    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private Grid terrainGrid;
    private int width;
    private int height;

    private List<TerrainFeatureData> terrainGridList;
    private List<TerrainFeatureData> openFeatures;
    private List<TerrainFeatureData> closedFeatures;

    public Pathfinding(TerrainGenerator terrainGenerator, Grid terrainGrid, int width, int height)
    {
        this.terrainGenerator = terrainGenerator;
        this.terrainGrid = terrainGrid;
        this.width = width;
        this.height = height;
        InitializeGrid();
    }

    public List<TerrainFeatureData> FindPath(int xStart, int yStart, int xEnd, int yEnd, bool allowDiagonal = false, int pathType = 1)
    {
        for (int i = 0; i < terrainGridList.Count; i++)
        {
            terrainGridList[i].m_gCost = int.MaxValue;
        }

        TerrainFeatureData startFeature = GetFeature(xStart, yStart);
        TerrainFeatureData endFeature = GetFeature(xEnd, yEnd);

        CalculateTerrainCosts(ref startFeature);
        CalculateTerrainCosts(ref endFeature);

        Debug.Log($"Generated start node at position {startFeature.xPos}, {startFeature.yPos} and end node at position {endFeature.xPos}, {endFeature.yPos}");


        openFeatures = new List<TerrainFeatureData> { startFeature };
        closedFeatures = new List<TerrainFeatureData>();

        startFeature.m_gCost = 0;
        if (!allowDiagonal)
        {
            startFeature.m_hCost = CalculateOrthogonalDistanceCost(startFeature, endFeature, pathType);
        } 
        else
        {
            startFeature.m_hCost = CalculateDiagonalDistanceCost(startFeature, endFeature, pathType);
        }
        startFeature.CalculateFCost();

        

        Debug.Log("Lists and data set");

        while(openFeatures.Count > 0)
        {
            TerrainFeatureData currentFeature = GetLowestCostTerrainFeature(openFeatures);
            openFeatures.Remove(currentFeature);
            closedFeatures.Add(currentFeature);

            if (currentFeature == endFeature)
            {
                Debug.Log("Path found!");
                return CalculatePath(currentFeature);
            }

            if(!allowDiagonal)
            {
                foreach (TerrainFeatureData neighborFeature in GetOrthogonalNeighbors(currentFeature))
                {
                    if (closedFeatures.Contains(neighborFeature)) continue;

                    int tentativeGCost = currentFeature.m_gCost + CalculateOrthogonalDistanceCost(currentFeature, neighborFeature, pathType);
                    //Debug.Log($"Tentative GCost for feature at position {neighborFeature.xPos}, {neighborFeature.yPos}: {tentativeGCost}");
                    if (tentativeGCost < neighborFeature.m_gCost || !openFeatures.Contains(neighborFeature))
                    {
                        
                        neighborFeature.m_gCost = tentativeGCost;
                        neighborFeature.m_hCost = CalculateOrthogonalDistanceCost(neighborFeature, endFeature, pathType);
                        neighborFeature.CalculateFCost();
                        neighborFeature.cameFromFeature = currentFeature;

                        if (!openFeatures.Contains(neighborFeature))
                        {
                            openFeatures.Add(neighborFeature);
                        }
                    }
                }
            }
            else
            {
                foreach(TerrainFeatureData neighborFeature in GetDiagonalNeighbors(currentFeature))
                {
                    if (closedFeatures.Contains(neighborFeature)) continue;

                    int tentativeGCost = currentFeature.m_gCost + CalculateDiagonalDistanceCost(currentFeature, neighborFeature, pathType);
                    if(tentativeGCost < neighborFeature.m_gCost)
                    {
                        neighborFeature.cameFromFeature = currentFeature;
                        neighborFeature.m_gCost = tentativeGCost;
                        neighborFeature.m_hCost = CalculateDiagonalDistanceCost(neighborFeature, endFeature, pathType);
                        neighborFeature.CalculateFCost();

                        if(!openFeatures.Contains(neighborFeature))
                        {
                            openFeatures.Add(neighborFeature);
                        }
                    }
                }
            }
        }

        Debug.Log("No path found!");
        return null;
    }


    private void InitializeGrid()
    {
        terrainGridList = new List<TerrainFeatureData>();

        for (int i = 0; i < terrainGenerator.width; i++)
        {
            for (int j = 0; j < terrainGenerator.height; j++)
            {
                TerrainFeatureData terrainFeature = new TerrainFeatureData(i, j);
                //Debug.Log("Calculating terrain features...");
                CalculateTerrainCosts(ref terrainFeature);

                terrainFeature.m_gCost = int.MaxValue;
                terrainFeature.CalculateFCost();
                terrainFeature.cameFromFeature = null;

                //Debug.Log($"Terrain feature initialized at {i}, {j} with river cost of {terrainFeature.m_riverCost} and with road cost of {terrainFeature.m_roadCost}");
                terrainGridList.Add(terrainFeature);
            }
        }
    }


    private List<TerrainFeatureData> CalculatePath(TerrainFeatureData endFeature)
    {
        List<TerrainFeatureData> path = new List<TerrainFeatureData>();
        TerrainFeatureData currentFeature = endFeature;

        path.Add(endFeature);

        while(currentFeature.cameFromFeature != null)
        {
            path.Add(currentFeature.cameFromFeature);
            currentFeature = currentFeature.cameFromFeature;
        }

        path.Reverse();
        return path;
    }


    private List<TerrainFeatureData> GetOrthogonalNeighbors(TerrainFeatureData currentFeature)
    {
        List<TerrainFeatureData> neighborFeatures = new List<TerrainFeatureData>();

        // Left Neighbor
        if(currentFeature.xPos - 1 >= 0)
        {
            //Debug.Log("Left neighbor found!");
            neighborFeatures.Add(GetFeature(currentFeature.xPos - 1, currentFeature.yPos));
        }

        // Right Neighbor
        if(currentFeature.xPos + 1 < terrainGenerator.width)
        {
            //Debug.Log("Right neighbor found!");
            neighborFeatures.Add(GetFeature(currentFeature.xPos + 1, currentFeature.yPos));
        }

        // Bottom Neighbor
        if(currentFeature.yPos - 1 >= 0)
        {
            //Debug.Log("Down neighbor found!");
            neighborFeatures.Add(GetFeature(currentFeature.xPos, currentFeature.yPos - 1));
        }

        // Top Neighbor
        if(currentFeature.yPos + 1 < terrainGenerator.height)
        {
            //Debug.Log("Up neighbor found!");
            neighborFeatures.Add(GetFeature(currentFeature.xPos, currentFeature.yPos + 1));
        }

        return neighborFeatures;
    }

    // TODO: Implement diagonal neighbor finding algorithm (Top left, top right, bottom left, bottom right)
    private List<TerrainFeatureData> GetDiagonalNeighbors(TerrainFeatureData currentFeature)
    {
        List<TerrainFeatureData> neighborFeatures = new List<TerrainFeatureData>();

        // Left Neighbor
        if (currentFeature.xPos - 1 >= 0)
        {
            neighborFeatures.Add(GetFeature(currentFeature.xPos - 1, currentFeature.yPos));
        }

        // Right Neighbor
        if (currentFeature.xPos + 1 < terrainGenerator.width)
        {
            neighborFeatures.Add(GetFeature(currentFeature.xPos + 1, currentFeature.yPos));
        }

        // Bottom Neighbor
        if (currentFeature.yPos - 1 >= 0)
        {
            neighborFeatures.Add(GetFeature(currentFeature.xPos, currentFeature.yPos - 1));
        }

        // Top Neighbor
        if (currentFeature.yPos + 1 < terrainGenerator.height)
        {
            neighborFeatures.Add(GetFeature(currentFeature.xPos, currentFeature.yPos + 1));
        }

        return neighborFeatures;
    }

    private TerrainFeatureData GetFeature(int x, int y)
    {
        foreach (TerrainFeatureData feature in terrainGridList)
        {
            if (feature.xPos == x && feature.yPos == y)
            {
                return feature;
            }
        }

        return null;
    }


    private int CalculateOrthogonalDistanceCost(TerrainFeatureData startPoint, TerrainFeatureData endPoint, int pathType)
    {
        int xDistance = Mathf.Abs(startPoint.xPos - endPoint.xPos);
        int yDistance = Mathf.Abs(startPoint.yPos - endPoint.yPos);
        int totalDistance = xDistance + yDistance;

        if (pathType == 0)
        {
            return MOVE_STRAIGHT_COST * totalDistance + endPoint.m_riverCost;
        }
        else if(pathType == 1)
        {
            return MOVE_STRAIGHT_COST * totalDistance + endPoint.m_roadCost;
        }
        else
        {
            return MOVE_STRAIGHT_COST * totalDistance;
        }

        
    }

    private int CalculateDiagonalDistanceCost(TerrainFeatureData startPoint, TerrainFeatureData endPoint, int pathType)
    {
        int xDistance = Mathf.Abs(startPoint.xPos - endPoint.xPos);
        int yDistance = Mathf.Abs(startPoint.yPos - endPoint.yPos);
        int remainingDistance = Mathf.Abs(xDistance - yDistance);
        
        if(pathType == 0)
        {
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remainingDistance + endPoint.m_riverCost;
        } 
        else if (pathType == 1)
        {
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remainingDistance + endPoint.m_roadCost;
        }
        else
        {
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remainingDistance;
        }
        
    }

    private void CalculateTerrainCosts(ref TerrainFeatureData passedFeature)
    {
        for (int i = 1; i < terrainGrid.transform.childCount; i++)
        {
            TerrainFeature layerFeature = terrainGenerator.SelectPathfindingTile(terrainGrid.transform.GetChild(i).GetComponent<Tilemap>(), passedFeature.xPos, passedFeature.yPos);
            if (layerFeature != null)
            {
                //Debug.Log($"Feature found on {terrainGrid.transform.GetChild(i).GetComponent<Tilemap>().name}");
                //Debug.Log("Layer river cost: " + layerFeature.riverCost);
                //Debug.Log("Layer road cost: " + layerFeature.roadCost);
                passedFeature.m_riverCost += layerFeature.riverCost;
                passedFeature.m_roadCost += layerFeature.roadCost;

                // return was added to make sure the most impactful terrain feature is the feature whose cost is taken.  In order to factor in
                // all terrain types, simply comment out return;
                return;
            }
        }

        if (passedFeature.m_riverCost == 0 && passedFeature.m_roadCost == 0)
        {
            passedFeature.m_riverCost = DEFAULT_RIVER_COST;
            passedFeature.m_roadCost = DEFAULT_ROAD_COST;
        }
    }

    private TerrainFeatureData GetLowestCostTerrainFeature(List<TerrainFeatureData> terrainFeatureListPassed)
    {
        TerrainFeatureData lowestCostTerrainFeature = terrainFeatureListPassed[0];
        for (int i = 1; i < terrainFeatureListPassed.Count; i++)
        {
            if (terrainFeatureListPassed[i].m_fCost < lowestCostTerrainFeature.m_fCost)
            {
                lowestCostTerrainFeature = terrainFeatureListPassed[i];
            }
        }

        return lowestCostTerrainFeature;
    }

}

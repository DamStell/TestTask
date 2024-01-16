 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public float nodeRadius;
    public Vector2 gridWorldSize;
    Node[,] grid;
    public List<Node> path { get; set; }
    float NodeDiameter { get { return nodeRadius * 2; } }
    int GridSizeX => Mathf.RoundToInt(gridWorldSize.x / NodeDiameter);
    int GridSizeY => Mathf.RoundToInt(gridWorldSize.y / NodeDiameter);

    private void Start()
    {
        CreateGrid();
    }
    void CreateGrid()
    {
        grid = new Node[GridSizeX, GridSizeY];
        Vector3 origin = transform.position - (GridSizeX / 2 * Vector3.right) - (GridSizeY / 2 * Vector3.forward);

        // Loop through each grid position and create nodes
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                Vector3 worldPos = origin + Vector3.right * (x * NodeDiameter + nodeRadius) + Vector3.forward * (y * NodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPos, nodeRadius, unwalkableMask);
                int movementPenalty = 0;
                grid[x, y] = new Node(worldPos, walkable, x, y, movementPenalty);
            }
        }
    }
    // Get a node at the specified grid coordinates
    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < GridSizeX && y >= 0 && y < GridSizeY)
        {
            return grid[x, y];
        }
        return null;
    }
    // Get a node from a world position
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.FloorToInt((GridSizeX) * percentX);
        int y = Mathf.FloorToInt((GridSizeY) * percentY);

        return GetNode(x, y);
    }
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Loop through neighboring positions
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                Node neighbor = GetNode(checkX, checkY);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(GridSizeX, 1, GridSizeY));

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.worldPos, Vector3.one * NodeDiameter);
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Node
{
    public Vector3 worldPos;
    public bool walkable;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public double GCost { get; set; }
    public double HCost { get; set; }
    public double FCost => GCost + HCost;

    public Node parent { get; set; }

    public Node(Vector3 worldPos, bool walkable, int gridX, int gridY, int movementPenalty)
    {
        this.worldPos = worldPos;
        this.walkable = walkable;
        this.gridX = gridX;
        this.gridY = gridY;
        this.movementPenalty = movementPenalty;
    }

    public double DistanceToNode(Node node)
    {
        float dx = Mathf.Abs(gridX - node.gridX);
        float dy = Mathf.Abs(gridY - node.gridY);

        return Mathf.Sqrt(2) * Mathf.Min(dx, dy) + Mathf.Abs(dx - dy);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public class AStarAlgorithm : MonoBehaviour
{
    public MapGrid grid;

    // Method to initialize the A* algorithm
    private void Awake()
    {
        if (grid == null)
        {
            Debug.LogError("MapGrid reference is null!");
        }
    }
    // Method to find a path using the A* algorithm
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            // Select the node with the lowest F cost
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }
            // Move the current node from open to closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }
            // For each neighbor of the current node
            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }
                double newCostToNeighbor = currentNode.GCost + currentNode.DistanceToNode(neighbor) + neighbor.movementPenalty;
                if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    // Update the costs and parent of the neighbor
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = neighbor.DistanceToNode(targetNode);
                    neighbor.parent = currentNode;

                    // If the neighbor is not in the open set, add it
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        Debug.LogError("Cannot find a path.");
    }
    // Method to retrace the found path from the target node to the start node
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Reverse the order of nodes to get the correct order from start to target
        path.Reverse();
        grid.path = path;
        return path;
    }
}


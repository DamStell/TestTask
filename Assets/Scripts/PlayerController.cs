using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public AStarAlgorithm pathfinding;
    public Transform leader;
    public float leaderMoveSpeed = 5.0f;
    public float verticalOffset = 0.5f;
    public float clickCooldown = 0.5f; // Minimalny czas pomiêdzy klikniêciami

    private float lastClickTime;
    private bool isMoving = false;

    private void Update()
    {
        if (!isMoving && Input.GetMouseButtonDown(0) && Time.time - lastClickTime > clickCooldown)
        {
            lastClickTime = Time.time;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                CalculateAndCheckPath(hit.point);
            }
        }
    }

    void CalculateAndCheckPath(Vector3 targetPosition)
    {
        pathfinding.grid.path = null; // Anuluj poprzedni¹ œcie¿kê
        pathfinding.FindPath(leader.position, targetPosition);

        if (pathfinding.grid.path != null && pathfinding.grid.path.Count > 0)
        {
            StartCoroutine(MoveLeaderAlongPath(pathfinding.grid.path));
        }
        else
        {
            Debug.LogError("Cannot find a path.");
        }
    }

    System.Collections.IEnumerator MoveLeaderAlongPath(System.Collections.Generic.List<Node> path)
    {
        isMoving = true;

        foreach (Node node in path)
        {
            Vector3 targetPosition = new Vector3(node.worldPos.x, node.worldPos.y + verticalOffset, node.worldPos.z);
            while (Vector3.Distance(leader.position, targetPosition) > 0.001f)
            {
                float step = leaderMoveSpeed * Time.deltaTime;
                leader.position = Vector3.MoveTowards(leader.position, targetPosition, step);
                yield return null;
            }
        }

        isMoving = false;
        pathfinding.grid.path = null; // Reset the path when the leader reaches the final position
    }
}


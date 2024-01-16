using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    public AStarAlgorithm pathfinding;
    public Transform leader;
    public float verticalOffset = 1.0f;
    public float clickCooldown = 0.5f; 
    private float lastClickTime;
    private bool isMoving = false;
    public CharacterStats stats;
    private float currentStamina;
    private bool isExhausted;

    private void Update()
    {
        if (!isMoving && Input.GetMouseButtonDown(0) && Time.time - lastClickTime > clickCooldown && !EventSystem.current.IsPointerOverGameObject())
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

    public void ChangeLeader(Transform newLeader)
    {
        leader = newLeader;
        StopAllCoroutines();
        StartCoroutine(MoveLeaderAlongPath(pathfinding.grid.path));
    }
    void CalculateAndCheckPath(Vector3 targetPosition)
    {
        pathfinding.grid.path = null; 
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
        if (path == null)
        {
            Debug.LogError("Path is null.");
            isMoving = false;
            yield break;
        }

        isMoving = true;

        foreach (Node node in path)
        {
            Vector3 targetPosition = new Vector3(node.worldPos.x, node.worldPos.y + verticalOffset, node.worldPos.z);
            while (Vector3.Distance(leader.position, targetPosition) > 0.001f)
            {
                float step = stats.speed * Time.deltaTime;
                if (!isExhausted)
                {
                    float raycastLength = 2.0f;
                    Ray ray = new Ray(leader.position, transform.forward);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, raycastLength))
                    {
                        if (hit.collider.CompareTag("Obstacle"))
                        {
                            // agility to reduce speed as you approach an obstacle
                            float distanceToObstacle = hit.distance;
                            float obstacleAvoidanceSpeed = Mathf.Lerp(stats.speed, stats.agility, distanceToObstacle / raycastLength) * Time.deltaTime;
                            float finalSpeed = Mathf.Min(step, obstacleAvoidanceSpeed);
                            // Decrease stamina depending on speed
                           // currentStamina = Mathf.Max(currentStamina - (finalSpeed / stats.speed) * stats.staminaConsumptionRate * Time.deltaTime, 0.0f);
                            leader.position = Vector3.MoveTowards(leader.position, targetPosition, finalSpeed);
                        }

                    }
                    else
                    {
                        // Unobstructed, move normally towards the point
                        leader.position = Vector3.MoveTowards(leader.position, targetPosition, step);  
                        if(isMoving)
                        {
                            currentStamina = Mathf.Max(currentStamina - stats.staminaConsumptionRate * Time.deltaTime, 0.0f);
                            if (currentStamina <= 0) isExhausted = true;
                        }else
                        {
                            currentStamina = Mathf.Min(currentStamina + stats.staminaRegenerationRate * Time.deltaTime, stats.maxStamina);
                        }
                    }
                }
                else
                {
                    //Leader is exhausted, reduce speed by half
                    float exhaustedSpeed = stats.speed * 0.5f * Time.deltaTime;
                    leader.position = Vector3.MoveTowards(leader.position, targetPosition, exhaustedSpeed);
                    currentStamina = Mathf.Min(currentStamina + stats.staminaRegenerationRate * Time.deltaTime, stats.maxStamina);
                    if (currentStamina >= stats.maxStamina) isExhausted = false;

                }
                yield return null;
            }
        }
        isMoving = false;
        pathfinding.grid.path = null; // Reset the path when the leader reaches the final position
    }
}
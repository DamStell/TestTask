﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Character : MonoBehaviour
{
    public AStarAlgorithm pathfinding;
    public Transform leader;
    public CharacterStats stats;
    public float distanceToLeader = 1.5f;
    public float recalculateInterval = 2.0f;
    public LayerMask obstacleLayer;
    private float currentStamina;
    private bool isExhausted;

    void Start()
    {
        if (pathfinding != null && leader != null && stats != null)
        {
            currentStamina = stats.maxStamina;
            isExhausted = false;
            StartCoroutine(FollowLeader());
            StartCoroutine(RecalculatePath());
        }
        else
        {
            Debug.LogError("Pathfinding, leader, or stats reference is null.");
        }
    }
    public void ChangeLeader(Transform newLeader)
    {
        leader = newLeader;
        StopAllCoroutines();
        StartCoroutine(FollowLeader());
        StartCoroutine(RecalculatePath());
    }
    IEnumerator FollowLeader()
    {
        while (true)
        {
            if (leader != null && transform != leader)
            {
                if (!isExhausted)
                {
                    float targetSpeed = Mathf.Min(stats.speed, leader.GetComponent<Character>().stats.speed);
                    Vector3 targetPosition = leader.position - leader.forward * distanceToLeader;
                    targetPosition.y = transform.position.y;
                    float raycastLength = 2.0f;
                    Ray ray = new Ray(transform.position, transform.forward);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, raycastLength))
                    {
                        if (hit.collider.CompareTag("Obstacle"))
                        {
                            // agility to reduce speed as you approach an obstacle
                            float distanceToObstacle = hit.distance;
                            float obstacleAvoidanceSpeed = Mathf.Lerp(stats.speed, stats.agility, distanceToObstacle / raycastLength) * Time.deltaTime;
                            float finalSpeed = Mathf.Min(targetSpeed, obstacleAvoidanceSpeed);
                            // Decrease stamina depending on speed
                            //currentStamina = Mathf.Max(currentStamina - (finalSpeed / stats.speed) * stats.staminaConsumptionRate * Time.deltaTime, 0.0f);
                            transform.position = Vector3.MoveTowards(transform.position, targetPosition, finalSpeed);
                        }
                    }
                    else
                    {
                        float step = targetSpeed * Time.deltaTime;
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                        currentStamina = Mathf.Max(currentStamina - stats.staminaConsumptionRate * Time.deltaTime, 0.0f);
                        //if (currentStamina <= 0) isExhausted = true;
                    }
                }
                else
                {
                    float exhaustedSpeed = stats.speed * 0.5f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, transform.position, exhaustedSpeed);
                    currentStamina = Mathf.Min(currentStamina + stats.staminaRegenerationRate * Time.deltaTime, stats.maxStamina);
                    if (currentStamina >= stats.maxStamina) isExhausted = false;
                }
            }

            yield return null;
        }
    }
    IEnumerator RecalculatePath()
    {
        while (true)
        {
            if (pathfinding != null && leader != null && pathfinding.grid != null)
            {
                pathfinding.FindPath(transform.position, leader.position);
                UpdatePathForAvoidance();
            }

            yield return new WaitForSeconds(recalculateInterval);
        }
    }
    void UpdatePathForAvoidance()
    {
        if (pathfinding.grid.path != null && pathfinding.grid.path.Count > 0)
        {
            for (int i = 0; i < pathfinding.grid.path.Count; i++)
            {
                Vector3 nodePosition = pathfinding.grid.path[i].worldPos;

                Collider[] colliders = new Collider[3];
                int count = Physics.OverlapSphereNonAlloc(nodePosition, 0.5f, colliders, obstacleLayer);

                for (int j = 0; j < count; j++)
                {
                    Collider collider = colliders[j];
                    if (collider != null && collider.transform != transform && collider.transform != leader)
                    {
 
                        if (IsLeaderApproaching(leader, transform, 1.0f))
                        {
                           
                            AvoidanceMove(leader, transform);
                        }
                    }
                }
            }
        }
    }
    bool IsLeaderApproaching(Transform leader, Transform follower, float distanceThreshold)
    {
        float distanceToLeader = Vector3.Distance(leader.position, follower.position);
        return distanceToLeader < distanceThreshold;
    }

    void AvoidanceMove(Transform leader, Transform follower)
    {
        Vector3 avoidanceDirection = (follower.position - leader.position).normalized;
        Vector3 avoidanceMove = Vector3.Cross(Vector3.up, avoidanceDirection);

        if (!Physics.Raycast(follower.position, avoidanceMove, 1.0f, obstacleLayer))
        {
            float avoidanceSpeed = 1.0f;
            follower.position += avoidanceMove * avoidanceSpeed * Time.deltaTime;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Character : MonoBehaviour
{
    public AStarAlgorithm pathfinding;
    public Transform leader;
    public float characterMoveSpeed = 3.0f;
    public float distanceToLeader = 1.5f;
    public float recalculateInterval = 2.0f;
    public LayerMask obstacleLayer;

    void Start()
    {
        if (pathfinding != null && leader != null)
        {
            StartCoroutine(FollowLeader());
            StartCoroutine(RecalculatePath());
        }
        else
        {
            Debug.LogError("Pathfinding or leader reference is null.");
        }
    }

    IEnumerator FollowLeader()
    {
        while (true)
        {
            Vector3 targetPosition = leader.position - leader.forward * distanceToLeader;
            targetPosition.y = transform.position.y;

            while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
            {
                if (Vector3.Distance(transform.position, leader.position) > distanceToLeader * 1.5f)
                {
                    float step = characterMoveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
                }

                yield return null;
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

                Collider[] colliders = new Collider[5]; // rozmiar na podstawie iloœci oczekiwanych postaci
                int count = Physics.OverlapSphereNonAlloc(nodePosition, 0.5f, colliders, obstacleLayer);

                for (int j = 0; j < count; j++)
                {
                    Collider collider = colliders[j];
                    if (collider != null && collider.transform != transform && collider.transform != leader)
                    {
                        Vector3 avoidanceDirection = (nodePosition - transform.position).normalized;
                        pathfinding.grid.path[i].worldPos = nodePosition + avoidanceDirection * 2.0f;
                    }
                }
            }
        }
    }
}

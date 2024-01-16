using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController playerController;
    public float smoothSpeed = 0.5f;
    public float rotationSpeed = 5.0f; 

    void LateUpdate()
    {
        if (playerController != null && playerController.leader != null)
        {
            Vector3 desiredPosition = playerController.leader.position + new Vector3(0f, 5f, -10f);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            Quaternion desiredRotation = Quaternion.LookRotation(playerController.leader.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
